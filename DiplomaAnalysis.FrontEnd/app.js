class App {
    static apiHost = 'http://localhost:7071';
    static apiServices =
        [
            '/api/Layout',
            '/api/References',
            '/api/Orthography2019',
            '/api/Runglish',
            '/api/WordingMisuse',
            '/api/CharReplacement',
            '/api/Punctuation',
            '/api/Pronouns',
            '/api/Table',
            '/api/Image'
        ];

    constructor() {
        this.dropArea = document.getElementById('drop-area');
        this.resultArea = document.getElementById('result-area');
        this.fileInput = document.getElementById('file');
        this.loader = document.getElementById('loader');
        this.results = [];
        this.initialPageTitle = document.title;
        this.registerEventHandlers();
    }

    registerEventHandlers() {
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            this.dropArea.addEventListener(eventName, this.preventDefaults, false);
        });

        ['dragenter', 'dragover'].forEach(eventName => {
            this.dropArea.addEventListener(eventName, this.highlightDropArea, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            this.dropArea.addEventListener(eventName, this.unhighlightDropArea, false);
        });

        ['drop'].forEach(eventName => {
            this.dropArea.addEventListener(eventName, this.handleDrop, false);
        });
    }

    preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    highlightDropArea(e) {
        app.dropArea.classList.add('highlight');
    }

    unhighlightDropArea(e) {
        app.dropArea.classList.remove('highlight');
    }

    handleDrop(e) {
        app.handleFiles(e.dataTransfer.files);
    }

    handleFiles(files) {
        if (files.length === 0) {
            return;
        }

        let file = files[0];
        let formData = new FormData();
        formData.append('file', file);

        this.resultArea.innerHTML = '';
        this.results = [];
        this.loader.style.display = 'block';
        this.setPageTitle(file.name);

        let responsesCount = 0;

        App.apiServices.forEach(async service => {
            try {
                let response = await fetch(App.apiHost + service, {
                    method: 'POST',
                    body: formData
                });

                if (response.ok) {
                    let json = await response.json();
                    app.processServiceResponse(json.length === 0 ? [{ code: 'SUCCESS01', extraMessage: service }] : json);
                }
                else {
                    app.processServiceResponse([{ code: response.status !== 400 ? 'ERR01' : 'ERR02', isError: true, extraMessage: service }]);
                }
            }
            catch {
                app.processServiceResponse([{ code: 'ERR01', isError: true, extraMessage: service }]);
            }

            if (++responsesCount == App.apiServices.length) {
                this.loader.style.display = 'none';
            }
        });

        this.fileInput.value = null;
    }

    processServiceResponse(data) {
        this.results = this.results.concat(data);

        data.forEach(message => {
            let ticket = ResultTicketFactory.create(message.code, message.extraMessage, message.isError);

            app.resultArea.appendChild(ticket);
        });
    }

    saveResultsToFile() {
        let blob = new Blob([JSON.stringify(this.results, null, 4)], { type: 'text/plain;charset=utf-8' });
        saveAs(blob, 'results.json');
    }

    loadResultsFromFile() {
        this.resultArea.innerHTML = '';
        this.results = [];

        let input = document.createElement('input');
        input.type = 'file';
        input.accept = '.json';
        input.onchange = async e => {
            let file = e.target.files[0];
            let json = JSON.parse(await file.text());

            app.processServiceResponse(json.length === 0 ? [{ code: 'SUCCESS01' }] : json);
            this.setPageTitle(file.name);
        };

        input.click();
    }

    setPageTitle(fileName) {
        document.title = `${this.initialPageTitle} - ${fileName}`;
    }
}

let app = new App();
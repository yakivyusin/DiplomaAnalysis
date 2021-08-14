class App {
    static apiHost = 'http://localhost:7071';
    static apiServices =
        [
            '/api/Layout',
            '/api/References',
            '/api/Orthography2019'
        ];

    constructor() {
        this.dropArea = document.getElementById('drop-area');
        this.resultArea = document.getElementById('result-area');
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

        App.apiServices.forEach(async service => {
            try {
                let response = await fetch(App.apiHost + service, {
                    method: 'POST',
                    body: formData
                });

                if (response.ok) {
                    let json = await response.json();
                    app.processServiceResponse(json.length === 0 ? [{ code: 'SUCCESS01' }] : json);
                }
                else {
                    app.processServiceResponse([{ code: response.status !== 400 ? 'ERR01' : 'ERR02', isError: true }]);
                }
            }
            catch {
                app.processServiceResponse([{ code: 'ERR01', isError: true }]);
            }
        });
    }

    processServiceResponse(data) {
        data.forEach(message => {
            let ticket = ResultTicketFactory.create(message.code, message.extraMessage, message.isError);

            app.resultArea.appendChild(ticket);
        });
    }
}

let app = new App();
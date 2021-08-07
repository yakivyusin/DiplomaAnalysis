class App {
    static apiHost = 'http://localhost:7071';
    static apiServices =
        [
            '/api/Layout',
            '/api/References'
        ];

    constructor() {
        this.dropArea = document.getElementById('drop-area');
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
        let file = files[0];
        let formData = new FormData();
        formData.append('file', file);

        App.apiServices.forEach(async service => {
            try {
                let response = await fetch(App.apiHost + service, {
                    method: 'POST',
                    body: formData
                });

                if (response.ok) {
                    app.processServiceResponse(await response.json());
                }
                else {
                    app.processServiceResponse([{ Code: 'ERR01', IsError: true }]);
                }
            }
            catch {
                app.processServiceResponse([{ Code: 'ERR01', IsError: true }]);
            }
        });
    }

    processServiceResponse(data) {
        console.log(data);
    }
}

let app = new App();
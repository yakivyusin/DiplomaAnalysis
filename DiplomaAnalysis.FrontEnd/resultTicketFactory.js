class ResultTicketFactory {
    static descriptions = {
        "ERR01": "Сталась непередбачена помилка підключення до сервісу аналізу. Спробуйте пізніше або зверніться до адміністратора.",
        "ERR02": "Ваш файл не було розпізнано як коректний .docx файл.",
        "REF01": "У вашому тексті не знайдено жодного посилання вигляду [1]. Для деяких із документів відсутність посилань дуже малоймовірна (напр., розділи 1-2 ПЗ).",
        "LAY01": "Всі документи оформлюються на папері розміру А4 (210 на 297 мм).",
        "LAY02": "Для більшості з документів ліве поле має становити 3 см, інші - по 2 см."
    };

    static create(code, extra, isError) {
        let div = document.createElement('div');
        div.classList.add('result-ticket', isError ? 'error' : 'warning');

        let codeParagraph = document.createElement('p');
        codeParagraph.classList.add('code');
        codeParagraph.innerText = code;

        let extraParagraph = document.createElement('p');
        extraParagraph.classList.add('extra');

        if (extra) {
            extraParagraph.innerText = extra;
        }

        let descriptionParagraph = document.createElement('p');
        descriptionParagraph.classList.add('description');
        descriptionParagraph.innerText = this.descriptions[code];

        div.appendChild(codeParagraph);
        div.appendChild(extraParagraph);
        div.appendChild(descriptionParagraph);

        return div;
    }
}
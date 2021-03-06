class ResultTicketFactory {
    static descriptions = {
        'SUCCESS01': 'Сервіс аналізу перевірив ваш файл та не знайшов жодних помилок.',
        'ERR01': 'Сталась непередбачена помилка підключення до сервісу аналізу. Спробуйте пізніше або зверніться до адміністратора.',
        'ERR02': 'Ваш файл не було розпізнано як коректний .docx файл сервісом аналізу.',
        'REF01': 'У вашому тексті не знайдено жодного посилання вигляду [1]. Для деяких із документів відсутність посилань дуже малоймовірна (напр., розділи 1-2 ПЗ).',
        'LAY01': 'Всі документи оформлюються на папері розміру А4 (210 на 297 мм).',
        'LAY02': 'Для більшості з документів ліве поле має становити 3 см, інші - по 2 см.',
        'ORT01': 'За <a href="https://bit.ly/3xMcJdt" target="_blank">новим правописом</a> треба писати: про<strong>є</strong>кт, а не проект; компонент "веб" разом з наступним словом, наприклад, <strong>веб</strong>сайт, а не веб сайт або веб-сайт.',
        'ORT02': 'Уникайте українсько-англійського суржику. Наприклад, не девайс, а пристрій; не нотифікація, а сповіщення.',
        'ORT03': 'Можливе використання слова у неправильному контексті. Деталі див. за <a href="misuse.txt" target="_blank">посиланням.</a>'
    };

    static create(code, extra, isError) {
        let template = document.importNode(document.getElementById('ticket-template'), true);
        let div = template.content.querySelector('div');
        let codeParagraph = template.content.querySelector('.code');
        let extraParagraph = template.content.querySelector('.extra');
        let descriptionParagraph = template.content.querySelector('.description');

        div.classList.add(code === 'SUCCESS01' ? 'success' : (isError ? 'error' : 'warning'));
        codeParagraph.innerHTML = code;
        extraParagraph.innerHTML = extra || '';
        descriptionParagraph.innerHTML = this.descriptions[code];

        return template.content;
    }
}
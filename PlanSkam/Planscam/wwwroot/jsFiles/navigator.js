function loadPage(uri) {
    sendAjax("GET", 'document', uri, function () {
        page = document.getElementById('page');
        page.innerHTML = request.response.body.innerHTML;
    });
}

function loadPage(uri, afterload) {
    sendAjax("GET", 'document', uri, function () {
        page = document.getElementById('page');
        page.innerHTML = request.response.body.innerHTML;
        if (afterload)
        afterload();
    });
}
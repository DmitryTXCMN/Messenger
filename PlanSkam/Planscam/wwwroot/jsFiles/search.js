function search() {
    input = document.getElementById('searchInput');
    sendAjax("GET", 'document', `/Home/Search?query=${input.value}`, function () {
        document.getElementById('searchReslutContainer').innerHTML = request.response.body.innerHTML;
    });
}
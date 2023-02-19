function likePlaylist(id) {
    btn = document.getElementById('likePlaylistBtn');
    if (btn.classList.contains('fi-rr-check')) {
        sendAjax("POST", 'document', `/Playlists/UnlikePlaylist?id=${id}`, function () {
            btn.classList.remove('fi-rr-check');
            btn.classList.add('fi-rr-plus');
            btn.innerHTML = "Add";
            updateLayoutPlaylists();
        });
    }
    else {
        sendAjax("POST", 'document', `/Playlists/LikePlaylist?id=${id}`, function () {
            btn.classList.remove('fi-rr-plus');
            btn.classList.add('fi-rr-check');
            btn.innerHTML = "In favorites";
            updateLayoutPlaylists();
        });
    }
}

//Хардкод, ибо не хватает времени
function likePlaylistHome(id) {
    btn = event.target;
    if (btn.classList.contains('fi-rr-heart')) {
        sendAjax("POST", 'document', `/Playlists/LikePlaylist?id=${id}`, function () {
            btn.classList.remove('fi-rr-heart');
            btn.classList.add('fi-sr-heart');
            updateLayoutPlaylists();
        });
    }
    else {
        sendAjax("POST", 'document', `/Playlists/UnlikePlaylist?id=${id}`, function () {
            btn.classList.remove('fi-sr-heart');
            btn.classList.add('fi-rr-heart');
            updateLayoutPlaylists();
        });
    }
}

function addTrackToPlaylistIndex(playlistId,trackId) {
    sendAjax("POST", "json", `/Playlists/AddTrackToPlaylist?playlistId=${playlistId}&trackId=${trackId}`, function () {
        document.getElementById(`playlist${playlistId}`).style.display = 'none';
    })
}

function updateLayoutPlaylists() {
    container = document.getElementById('layoutPlaylistsContainer');
    sendAjax("GET", 'document', '/Playlists/LayoutPlaylists/', function () {
        container.innerHTML = request.response.body.innerHTML;
        updateStudioTracklist(); //Если открыто студио (небольшой костыль, чтобы сэкономить время)
    })
}

function updateStudioTracklist() {
    container = document.getElementById('studioTracklistContainer');
    if (container) {
        sendAjax("GET", 'document', '/Tracks/GetOwned/', function () {
            container.innerHTML = request.response.body.innerHTML;
        })
    }
}

function addTrackToFavourite(id) {
    btn = document.getElementById(`trackFavBtn${id}`);

    if (btn.classList.contains('fi-sr-heart'))
        sendAjax("POST", 'json', `/Tracks/RemoveTrackFromFavourite/${id}`, function () {
            if (id == JSON.parse(localStorage.playlist).trackIds[localStorage.nowPlayed]) {
                setFavourite(!btn.classList.contains('fi-sr-heart'));
            }
            setBtnFavourite(btn, false);
        });
    else
        sendAjax("POST", 'json', `/Tracks/AddTrackToFavourite/${id}`, function () {
            if (id == JSON.parse(localStorage.playlist).trackIds[localStorage.nowPlayed]) {
                setFavourite(!btn.classList.contains('fi-sr-heart'));
            }
            setBtnFavourite(btn, true);
        });
}

function setBtnFavourite(btn, isLiked) {
    if (isLiked) {
        btn.classList.remove('fi-rr-heart');
        btn.classList.add('fi-sr-heart');
    }
    else {
        btn.classList.remove('fi-sr-heart');
        btn.classList.add('fi-rr-heart');
    }
}

function removeTrackFromPlaylist(playlistId, trackId) {
    trackElement = document.getElementById(`track${trackId}`);
    sendAjax("POST", "json", `/Playlists/RemoveTrackFromPlaylist?playlistId=${playlistId}&trackId=${JSON.parse(localStorage.playlist).trackIds[localStorage.nowPlayed]}`, function () {
        trackElement.style.display = 'none';
    })
}

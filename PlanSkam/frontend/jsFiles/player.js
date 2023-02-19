const 
    artist = document.getElementById('artist'),
    playButton = document.getElementById('play'),
    audio = document.getElementById('audio');
    progressBar = document.getElementById('progress');
    progressContainer = document.getElementById('progeressContainer');
    playingNow = false;

function play() {
    if (!playingNow)
    {
        playButton.classList.remove('fi-rr-play');
        playButton.classList.add('fi-rr-pause');
        audio.play();
    }
    else
    {
        playButton.classList.remove('fi-rr-pause');
        playButton.classList.add('fi-rr-play');
        audio.pause();
    }
    playingNow = !playingNow;
}

function testFunc() {
    artist.innerHTML = "test complete";
}

function progressBarUpdate(e) {
    let {duration, currentTime} = e.srcElement;
    progressBar.style.width = `${currentTime/duration*100}%`;
}

audio.addEventListener('timeupdate',progressBarUpdate);

function setProgress(e) {
    audio.currentTime = e.offsetX/progressContainer.clientWidth*audio.duration;
}

progressContainer.addEventListener('click',setProgress);
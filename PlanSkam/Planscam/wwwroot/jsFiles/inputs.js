function imgInput() {
    imgInput = document.getElementById('imgInput');
    imgLabel = document.getElementById('imgInputLabel');

    function updateValue() {
        imgLabel.innerHTML = 'Loaded <i style="display: flex" class="fi fi-rr-check"></i>'
    }

    imgInput.addEventListener('input', updateValue)
}

function mp3Input() {
    mp3Input = document.getElementById('mp3Input');
    mp3Label = document.getElementById('mp3InputLabel');

    function updateValue() {
        mp3Label.innerHTML = 'Loaded <i style="display: flex" class="fi fi-rr-check"></i>'
    }

    mp3Input.addEventListener('input', updateValue)
}
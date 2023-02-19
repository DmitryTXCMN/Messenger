let labels1 = document.getElementsByTagName("label");
let inputs1 = document.getElementsByTagName("input");
let labels = Array.prototype.slice.call(labels1);
let inputs = Array.prototype.slice.call(inputs1);
let inputsHoverable = [];

inputs.forEach(function (input) {
    labels.forEach(function (label) {
        if (label.htmlFor == input.id && (input.type == 'text' || input.type == 'password')) {
            input.label = label;
            inputsHoverable.push(input);
        }
    });
});

function updateValue(input) {
    if (input.value == '')
        input.label.style.display = 'flex';
    else
        input.label.style.display = 'none';
}

let inputsHoverable1 = Array.prototype.slice.call(inputsHoverable);
inputsHoverable1.forEach(function (input) {
    input.addEventListener('input', () => updateValue(input));
    updateValue(input);
})
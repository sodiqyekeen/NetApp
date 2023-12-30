var otpInputs = [];

window.registerOtpFormFocusHanlder = () => {
    otpInputs = Array.prototype.slice.call(document.querySelectorAll('input[type=text]'));
    otpInputs.forEach((input, index) => {
        input.addEventListener("keyup", (e) => {
            window.handleElementFocus(e.key, input, index);

        });
    });
}

window.handleElementFocus = (key, element, index) => {
    if (element.value) {
        var nextInput = otpInputs[index + 1];
        if (nextInput) {
            nextInput.focus();
        }
        else {
            element.blur();
        }
    }
    else if (key === "Backspace") {
        var previousInput = otpInputs[index - 1];
        if (previousInput) {
            previousInput.focus();
        }
    }
};

window.unregisterOtpFormFocusHanlder = () => {
    otpInputs.forEach((input, index) => {
        input.removeEventListener("keyup", (e) => {
            window.handleElementFocus(e.key, input, index);
        });
    });
}


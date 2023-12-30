/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./**/*.{razor,html}",
        "./node_modules/flowbite/**/*.js"
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require('flowbite/plugin')
    ],
}


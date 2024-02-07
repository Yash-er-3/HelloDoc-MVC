const modebtn = document.getElementById('img-btn')
const imgbtn = document.getElementById('img-mode')
const font = document.getElementById('font-color')
const contentcolor = document.getElementById('content-color')
const dateIcon = document.getElementById('date-icon')
modebtn.addEventListener('click', darkLight)
var flag;

window.onload = function () {
    var array = document.cookie.split("=");
    // console.log(array)
    flag = parseInt(array[1]);
    darkLight()
    // console.log(flag)
    try {
        const myModal = new bootstrap.Modal(document.getElementById('exampleModal'));

        myModal.show();

        const closeBtn = document.getElementById("closeBtn");
        closeBtn.addEventListener('click', function () { myModal.hide(); })
    } catch (error) {

    }
}

function darkLight() {
    if (flag == 0) {
        document.querySelector('body').style.backgroundColor = "black"
        imgbtn.toString().replace('light', 'dark')
        imgbtn.src = "../images/dark.png"
        document.querySelector('body').style.color = "white"
        try {
            contentcolor.style.backgroundColor = "rgb(33,37,41)"
            dateIcon.classList.replace('date-iconLight', 'date-iconDark')
        }
        catch {

        }
        document.cookie = "flag = " + flag;
        flag = 1;
    }
    else {
        document.querySelector('body').style.backgroundColor = "rgb(247,247,247)"
        document.querySelector('body').setAttribute('data-bs-theme', 'light')
        imgbtn.toString().replace('dark', 'light')
        imgbtn.src = "../images/light.png"
        document.querySelector('body').style.color = "black"
        try {
            contentcolor.style.backgroundColor = "white"
            dateIcon.classList.replace('date-iconDark', 'date-iconLight')
        }
        catch {

        }
        

        // font.style.color = "white"
        document.cookie = "flag = " + flag
        flag = 0;
    }
}

// file upload js

//const actualBtn = document.getElementById('actual-btn');

//const fileChosen = document.getElementById('file-chosen');

//actualBtn.addEventListener('change', function () {
//    fileChosen.textContent = this.files[0].name
//})
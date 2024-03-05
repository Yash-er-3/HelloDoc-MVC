var modebtn = document.getElementById('img-btn')
var imgbtn = document.getElementById('img-mode')
var terms = document.getElementById('terms-1')

modebtn.addEventListener('click', darkLight)

var flag;

window.onload = function () {
    var array = document.cookie.split("=");
    console.log(array)
    flag = parseInt(array[1]);
    darkLight()
    // console.log(flag)
}

function darkLight() {
    if (flag == 0) {
        document.querySelector('body').setAttribute('data-bs-theme', 'dark')
        imgbtn.toString().replace('light', 'dark')
        imgbtn.src = "../images/light.png"

        document.cookie = "flag = " + flag;
        flag = 1;
    }
    else {
        document.querySelector('body').setAttribute('data-bs-theme', 'light')
        imgbtn.toString().replace('dark', 'light')
        imgbtn.src = "../images/dark.png"
        //terms.style.color = "gray"
        document.cookie = "flag = " + flag
        flag = 0;
    }
}

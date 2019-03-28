// function myFunction() {
//     document.getElementById("myDropdown").classList.toggle("show");
//   }
//   window.onclick = function(event) {
//     if (!event.target.matches('.dropbtn')) {
//       var dropdowns = document.getElementsByClassName("dropdown-content");
//       var i;
//       for (i = 0; i < dropdowns.length; i++) {
//         var openDropdown = dropdowns[i];
//         if (openDropdown.classList.contains('show')) {
//           openDropdown.classList.remove('show');
//         }
//       }
//     }
//   }

$(document).ready(function () {
    $(".row1").click(function () {
        var array = this.className.split(" ");
        var id = array[array.length - 1];
        alert(id);
        $("#" + id).toggle();
    });
    //$("#row2").click(function () {
    //    $(".title-down-row.row2").toggle();
    //});
    //$("#row3").click(function () {
    //    $(".title-down-row.row3").toggle();
    //});
    //$("#row4").click(function () {
    //    $(".title-down-row.row4").toggle();
    //});
    //$("#row5").click(function () {
    //    $(".title-down-row.row5").toggle();
    //});
    //$("#row6").click(function () {
    //    $(".title-down-row.row6").toggle();
    //});
    //$("#row7").click(function () {
    //    $(".title-down-row.row7").toggle();
    //});
    //$("#row8").click(function () {
    //    $(".title-down-row.row8").toggle();
    //});
    //$("#row9").click(function () {
    //    $(".title-down-row.row9").toggle();
    //});
    //$("#row10").click(function () {
    //    $(".title-down-row.row10").toggle();
    //});
});

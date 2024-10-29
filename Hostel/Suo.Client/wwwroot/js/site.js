// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function OpenWindow(e) {
    document.getElementById("ClaimTypeId").value = e.currentTarget.dataset.ClaimTypeId;
    document.getElementById("ClaimTemplateId").value = e.currentTarget.dataset.ClaimTemplateId;

    if (e.currentTarget.dataset.IsEnableTimetostart == "True") {
        document.getElementById("TimeToStartI").setAttribute("required", "");
        document.getElementById("TimeToStart").style.display = "block";
    } else {
        document.getElementById("TimeToStartI").removeAttribute("required");
        document.getElementById("TimeToStart").style.display = "none";
    }

    if (e.currentTarget.dataset.IsEnableTimetoend == "True") {
        document.getElementById("TimeToEndI").setAttribute("required", "");
        document.getElementById("TimeToEnd").style.display = "block";
    } else {
        document.getElementById("TimeToEndI").removeAttribute("required");
        document.getElementById("TimeToEnd").style.display = "none";
    }

    if (e.currentTarget.dataset.IsEnableTimetogo == "True") {

        document.getElementById("TimeToGoI").setAttribute('required', '');
        //document.getElementById("TimeToGoI").required = true;
        document.getElementById("TimeToGo").style.display = "block";
    } else {
        document.getElementById("TimeToGoI").removeAttribute("required");
        document.getElementById("TimeToGo").style.display = "none";
    }

    if (e.currentTarget.dataset.IsEnableReason == "True") {

        document.getElementById("ReasonI").setAttribute('required', '');
        //document.getElementById("TimeToGoI").required = true;
        document.getElementById("Reason").style.display = "block";

    } else {
        document.getElementById("ReasonI").removeAttribute("required");
        document.getElementById("Reason").style.display = "none";
    }
}
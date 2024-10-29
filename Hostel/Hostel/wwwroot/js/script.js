

function StartDraggbleItems() {
    $(".dragroomitem").draggable({ /*revert: true,*/ containment: "#draganddropcontainer", appendTo: "#draganddropcontainer", helper: 'clone', zIndex: 110,
       
        start: function (event, ui) {
            var w = $(this).css('width');
            var h = $(this).css('height');
            ui.helper.css('width', w).css('height', h);
        }

    });
    let ex = 3;
    $(".dropitem").droppable({
        accept: ".dragroomitem",
        //classes: {
        //    "ui-droppable-active": "ui-state-active",
        //    "ui-droppable-hover": "ui-state-hover"
        //},
        drop: function (event, ui) {
            let roomNumber = ui.helper[0].dataset.room;
            let roomid = ui.helper[0].dataset.roomid;
            $(this)
                .addClass("ui-state-highlight")
                .find("p")
                //.html("Dropped!");
                .html(roomNumber);
            this.dataset.roomid = roomid;
            this.dataset.roomnumber = roomNumber;
            $("#removebtn" + this.id).removeClass("displaynone");
        }
    });

}

function GetRoomIdInDutyTable(id)
{
    let el = $("#" + id);
    return el[0].dataset.roomid;
}
function GetRoomNumberInDutyTable(id) {
    let el = $("#" + id);
    return el[0].dataset.roomnumber;
}
function RemoveItemInDisplay(id)
{
    $("#" + id).removeClass("ui-state-highlight").find("p").empty();
    let el = $("#" + id);
    el[0].dataset.roomid = "";
    el[0].dataset.roomnumber = "";
    $("#removebtn" + id).addClass("displaynone");
}

function savePDF(name) {
    let element = document.getElementById('PrintingTable');

    var opt = {
        //margin: 1,
        filename: name + '.pdf',
        //image: { type: 'jpeg', quality: 0.98 },
        //html2canvas: { scale: 2 },
        jsPDF: { unit: 'in', format: 'a4', orientation: 'landscape' }
    };


    html2pdf().set(opt)
        .from(element)
        .save();

}
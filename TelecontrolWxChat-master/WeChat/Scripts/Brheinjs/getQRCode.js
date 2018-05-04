$(function () {
    //生成
    $(".vjbutton_txtR").click(function () {
        createImage();
    });
});
//生成二维码
function createImage() {
    var content = $('#page7_jMemo1').text();
    var frist = content.substring(0, 1);
    var last = content.substring(content.length - 1, content.length);
    if (last == ";") {
        content = content.substring(0, content.length - 1);
    }
    var arrStr = content.split(';');
    if (arrStr.length <= 1) {
        if (content == "") {
            $('#page7_jImages3').empty();
            $('#page7_jImages3').append('<p id="ErrorTest"></p>');
            $("#ErrorTest").html("参数为空！");
            return false;
        }
        if (!(frist == "0" || frist == "1" || frist == "2" || frist == "3" || frist == "4" || frist == "5" || frist == "6")) {
            $('#page7_jImages3').empty();
            $('#page7_jImages3').append('<p id="ErrorTest"></p>');
            $("#ErrorTest").html("请输入有效参数！");
            return false;
        }
        //ajax开始
        $.post("/AdminQRCode/GetORImage/",
            { "content": content },
            function (data) {
                $('#page7_jImages3').empty();
                $('#page7_jImages3').append('<img id="ORImage" width="200px" height="200px" />');
                $('#page7_jImages3').append('<br /><label id="_label"></label>');
                $("#ORImage").attr("src", data.imageUrl);
                $("#_label").text(data.QRName);
            });
        //ajax结束
    } else {
        var error = "";
        for (var i = 0; i < arrStr.length; i++) {
            var first = arrStr[i].substring(0, 1);
            if (first == "" || !(frist == "0" || frist == "1" || frist == "2" || frist == "3" || frist == "4" || frist == "5" || frist == "6")) {
                error = "参数有误！";
            }
        }
        if (error == "") {
            $.post("/AdminQRCode/GetORImageList/",
                { "arr": arrStr },
                function (data) {
                    debugger;
                    $('#page7_jImages3').empty();
                    for (var i = 0; i < arrStr.length; i++) {
                        $('#page7_jImages3').append('<div style="float:left;margin-left:40px;margin-bottom:20px"><img id="ORImage_' + i + '" width="160px" height="160px"/><br/><label id="_label_' + i + '"></label></div>');
                        $("#ORImage_" + i).attr("src", data.data[arrStr[i]].Data.imageUrl);
                        $("#_label_" + i).text(data.data[arrStr[i]].Data.QRName);
                    }
                });
        } else {
            $('#page7_jImages3').empty();
            $('#page7_jImages3').append('<p id="ErrorTest"></p>');
            $("#ErrorTest").html("请确认参数是否有误！");
            return false;
        }
    }
}

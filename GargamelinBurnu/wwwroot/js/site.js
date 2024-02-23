var reqFunc = function(textValue,no)
{
    $("#spinner").show();
    
    if (no == undefined) {
        $("#spinner").hide();
        $("#DivError").append('Bir şeyler ters gitti !'+
        '<br>');
        setTimeout(function() {
            $("#DivError").empty();
        }, 10000); // 30 saniye
        return;
    }

    var no = parseInt(no);

    
    if (typeof textValue !== "string") {
        $("#spinner").hide();
        $("#DivError").append('Yorum değeri yanlış !'+
                                                              '<br>');
        return;
    }
    
    
    

    else if (textValue.trim() === ''){
        $("#spinner").hide();
        $("#DivError").append('Yorum boş gidemez !'+
                                                  '<br>');
        return;
    }

    else{
        textValue = textValue.trim();

        $.ajax({

            type: 'POST',
            url: '/Subject/addComment/',
            dataType: 'json',
            data: {
                SubjectId: no,
                Text: textValue
            },
            success: function(yorum) {
                $("#spinner").hide();

                var date = new Date(yorum.createdAt);
                var shortDate = date.toLocaleDateString('tr-TR');



                $("#comments").append(`
                <div class="d-flex flex-column my-2" id="comment-container">
                    <div class="bg-white">
                        <div class="flex-row d-flex">
                            <img src="dp.jpg" width="40" class="rounded-circle">
                            <div class="d-flex flex-column justify-content-start ml-2">
                                <span class="d-block font-weight-bold name">${yorum.username}</span>
                                <span class="date text-black-50">Katılma Tarihi : ${shortDate}</span>
                                <span class="date text-black-50">Mesaj Sayısı : ${yorum.messageCount}</span>
                            </div>
                        </div>
                        <div class="mt-3">
                            <p class="comment-text">${yorum.text}</p>
                        </div>
                    </div>
                    <div class="bg-white">
                        <div class="text-muted">Hemen şimdi</div>
                    </div>
                </div>
            `);



                $("#Text").val('');

                var adet = parseInt($("#commentCount").text());
                $("#commentCount").text(adet + 1);
            },
            error: function(xhr, status, error) {
                $("#spinner").hide();

                $("#DivError").append('Bir hata oluştu'+
                    '<br>');

                setTimeout(function() {
                    $("#DivError").empty();
                }, 10000);
            }


        });
    }
}



var ldhFunc = function (no,str){
    
    $("#spinner").show();

    if (no == undefined) {
        $("#spinner").hide();
        $("#err").append(`Önce <a href="/account/login" style="text-decoration=none">giriş yap</a> !`
            + '<br>');
        return;
    }


    var no = parseInt(no);

    if (typeof str !== "string") {
        $("#spinner").hide();
        $("#DivError").append('Bir şeyler ters gitti type error !'+
            '<br>');
        return;
    }


    else if (str.trim() === ''){
        $("#spinner").hide();
        $("#DivError").append('Bir şeyler ters gitti !!'+
            '<br>');
        return;
    }

    
    else{
        if(str.includes("_like")){
            $.ajax({
                type : 'Post',
                url : '/subject/likeSubject',
                dataType : 'json',
                data:{
                    SubjectId: no
                },
                success : function (ret){
                    $("#spinner").hide();
                    
                    if (ret.success == -1){
                        $("#err").append('Bir hata oluştu');
                        $("#like_icon").removeClass("text-primary");
                    }
                    

                    if (ret.success == 1){
                        $("#dislike_icon").removeClass("text-danger");
                        $("#heart_icon").removeClass("text-warning");

                        $("#heartcount").text(ret.heartcount);
                        $("#likecount").text(ret.likecount);
                        $("#dislikecount").text(ret.dislikecount);
                    }
                    else if (ret.success == 2){
                        $("#like_icon").removeClass("text-primary");
                        $("#dislike_icon").removeClass("text-danger");
                        $("#heart_icon").removeClass("text-warning");

                        $("#heartcount").text(ret.heartcount);
                        $("#likecount").text(ret.likecount);
                        $("#dislikecount").text(ret.dislikecount);
                    }

                    return false;
                },
                error:function (xhr, status, error){
                    $("#spinner").hide();
                    $("#err").append('Bir hata oluştu'
                        +'<br>');
                    setTimeout(function() {
                        $("#err").empty();
                    }, 30000);
                }
            });
        }
        else if(str.includes("dislike")){
            $.ajax({
                type : 'Post',
                url : '/subject/dislikeSubject',
                dataType : 'json',
                data:{
                    SubjectId: no
                },
                success : function (ret){

                    $("#spinner").hide();
                    

                    if (ret.success == -1){
                        $("#err").append('Bir hata oluştu');
                        $("#dislike_icon").removeClass("text-danger");
                    }
                    
                   
                    
                    
                    if (ret.success == 1){
                        
                        $("#dislike_icon").addClass("text-danger");
                        $("#like_icon").removeClass("text-primary");
                        $("#heart_icon").removeClass("text-warning");

                        $("#heartcount").text(ret.heartcount);
                        $("#likecount").text(ret.likecount);
                        $("#dislikecount").text(ret.dislikecount);
                    }
                    else if (ret.success == 2){
                        $("#dislike_icon").removeClass("text-danger");
                        $("#like_icon").removeClass("text-primary");
                        $("#heartcount").removeClass("text-warning");

                        $("#heartcount").text(ret.heartcount);
                        $("#likecount").text(ret.likecount);
                        $("#dislikecount").text(ret.dislikecount);
                    }

                    return false;
                },
                error:function (xhr, status, error){
                    $("#spinner").hide();
                    $("#err").append('Bir hata oluştu'
                        +'<br>');
                    setTimeout(function() {
                        $("#err").empty();
                    }, 30000);
                }
            });
        }
        else if(str.includes("heart")){
            $.ajax({
                type : 'Post',
                url : '/subject/heartSubject',
                dataType : 'json',
                data:{
                    SubjectId: no
                },
                success : function (ret){
                    $("#spinner").hide();
                    if (ret.success == -1){
                        $("#err").append('Bir hata oluştu');
                        $("#heart_icon").removeClass("text-warning");
                    }
                    

                    if (ret.success == 1){
                        $("#heart_icon").addClass("text-warning");
                        $("#like_icon").removeClass("text-primary");
                        $("#dislike_icon").removeClass("text-danger");

                        $("#heartcount").text(ret.heartcount);
                        $("#likecount").text(ret.likecount);
                        $("#dislikecount").text(ret.dislikecount);
                    }
                    else if (ret.success == 2){
                        $("#heart_icon").removeClass("text-warning");
                        $("#like_icon").removeClass("text-primary");
                        $("#dislike_icon").removeClass("text-danger");

                        $("#heartcount").text(ret.heartcount);
                        $("#likecount").text(ret.likecount);
                        $("#dislikecount").text(ret.dislikecount);
                    }

                    return false;
                },
                error:function (xhr, status, error){
                    $("#spinner").hide();
                    $("#err").append('Bir hata oluştu'
                        +'<br>');
                    setTimeout(function() {
                        $("#err").empty();
                    }, 15000);
                }
            });
        }
    }
    
}

var ComLDFunc = function(clickedId){
    $("#spinner").show();
    
    if(clickedId == undefined){
        $(`#err`).append('Bir hata oluştu');
        setTimeout(function() {
            $("#err").empty();
        }, 15000);
        return;
    }

    var commentId = clickedId.split("-")[0].split("_")[1];

    if(commentId == undefined){
        $("#err").append(`Önce <a href="/account/login" style="text-decoration=none">giriş yap</a> !`
            + '<br>');
        setTimeout(function() {
            $("#err").empty();
        }, 15000);
        return;
    }

    if (clickedId.includes("_like")) {
        $("#spinner").show();
        $.ajax({
            type: 'Post',
            url: '/comment/LikeComment',
            dataType: 'json',
            data: {
                CommentId: commentId
            },
            success: function(ret) {
                $("#spinner").hide();
                
                
                if (ret.success == -1) {
                    $(`#err_${commentId}-400`).append('Bir hata oluştu');
                    $(`#like_icon_${commentId}-445`).removeClass("text-primary");
                } else if (ret.success == 1) {
                    $(`#like_icon_${commentId}-445`).addClass("text-primary");
                    $(`#dislike_icon_${commentId}-854`).removeClass("text-danger");

                    $(`#like_count_${commentId}-878`).text(ret.likecount);
                    $(`#dislike_count_${commentId}-143`).text(ret.dislikecount);
                } else if (ret.success == 2) {
                    $(`#like_icon_${commentId}-445`).removeClass("text-primary");
                    $(`#dislike_icon_${commentId}-854`).removeClass("text-danger");

                    $(`#like_count_${commentId}-878`).text(ret.likecount);
                    $(`#dislike_count_${commentId}-143`).text(ret.dislikecount);
                }

                return false;
            },
            error: function(xhr, status, error) {
                $("#spinner").hide();
                $(`#err_${commentId}-400`).append('Bir hata oluştu' + '<br>');
                setTimeout(function() {
                    $(`#err_${commentId}-400`).empty();
                }, 30000);
            }
        });
    }
    else if(clickedId.includes("dislike")){
        $("#spinner").show();
        $.ajax({
            type: 'Post',
            url: '/comment/dislikeComment',
            dataType: 'json',
            data: {
                CommentId: commentId
            },
            success: function(ret) {
                $("#spinner").hide();
               
                if (ret.success == -1) {
                    $(`#err_${commentId}-400`).append('Bir hata oluştu');
                    $(`#dislike_icon_${commentId}-854`).removeClass("text-danger");
                } else if (ret.success == 1) {
                    $(`#dislike_icon_${commentId}-854`).addClass("text-danger");
                    $(`#like_icon_${commentId}-445`).removeClass("text-primary");

                    $(`#like_count_${commentId}-878`).text(ret.likecount);
                    $(`#dislike_count_${commentId}-143`).text(ret.dislikecount);
                } else if (ret.success == 2) {
                    $(`#like_icon_${commentId}-445`).removeClass("text-primary");
                    $(`#dislike_icon_${commentId}-854`).removeClass("text-danger");

                    $(`#like_count_${commentId}-878`).text(ret.likecount);
                    $(`#dislike_count_${commentId}-143`).text(ret.dislikecount);
                }

                return false;
            },
            error: function(xhr, status, error) {
                $("#spinner").hide();
                $(`#err_${commentId}-400`).append('Bir hata oluştu' + '<br>');
                setTimeout(function() {
                    $(`#err_${commentId}-400`).empty();
                }, 15000);
            }
        });
    }
}
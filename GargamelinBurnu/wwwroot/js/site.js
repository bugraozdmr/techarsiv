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
                console.log(yorum)

                var date = new Date(yorum.createdAt);
                var shortDate = date.toLocaleDateString('tr-TR');

                var userSignatureHTML = '';
                if (yorum.userSignature != null) {
                    userSignatureHTML = `
                        <hr/>
                        <div>
                            <p style="color: gray; font-size: 12px; margin-bottom: 5px;">Kullanıcı Imzası:</p>
                            <p style="font-size: 14px; margin-bottom: 0;">${yorum.userSignature}</p>
                        </div>
                        `;
                };



                $("#comments").append(`
                <div class="d-flex flex-column my-2" id="comment-container">
                    <div class="bg-white">
                        <div class="flex-row d-flex">
                            <img src="${yorum.userImage}" style="width: 40px;height: 40px" class="rounded-circle mt-2">
                            <div class="d-flex flex-column justify-content-start ml-2 mx-3">
                                <span class="d-block font-weight-bold name"><a
                                 href="https://localhost:7056/biri/${yorum.username}"
                                 style="text-decoration: none">${yorum.username}</a></span>
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
                    ${userSignatureHTML}
                </div>
            `);



                $('.ck-content p').Text('');

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




// CkEditor

class MyUploadAdapter {
    constructor( loader ) {
        this.loader = loader;
    }


    upload() {
        return this.loader.file
            .then( file => new Promise( ( resolve, reject ) => {
                this._initRequest();
                this._initListeners( resolve, reject, file );
                this._sendRequest( file );
            } ) );
    }


    abort() {
        if ( this.xhr ) {
            this.xhr.abort();
        }
    }


    _initRequest() {
        const xhr = this.xhr = new XMLHttpRequest();

        xhr.open( 'POST', '/subject/UploadImage', true );
        xhr.responseType = 'json';
    }

    _initListeners( resolve, reject, file ) {
        const xhr = this.xhr;
        const loader = this.loader;
        const genericErrorText = `Couldn't upload file: ${ file.name }.`;

        xhr.addEventListener( 'error', () => reject( genericErrorText ) );
        xhr.addEventListener( 'abort', () => reject() );
        xhr.addEventListener( 'load', () => {
            const response = xhr.response;

            if ( !response || response.error ) {
                return reject( response && response.error ? response.error.message : genericErrorText );
            }

            resolve( {
                default: response.url
            } );
        } );

        if ( xhr.upload ) {
            xhr.upload.addEventListener( 'progress', evt => {
                if ( evt.lengthComputable ) {
                    loader.uploadTotal = evt.total;
                    loader.uploaded = evt.loaded;
                }
            } );
        }
    }

    _sendRequest( file ) {
        const data = new FormData();

        data.append( 'upload', file );


        this.xhr.send( data );
    }

}

function MyCustomUploadAdapterPlugin( editor ) {
    editor.plugins.get( 'FileRepository' ).createUploadAdapter = ( loader ) => {

        return new MyUploadAdapter( loader );
    };
}


// preview dummy function
var dummy = function (){
    $(document).ready(function(){
        $('#preview').click(function(){
            var content = editor.getData();
            var title = $('#Title').val();

            $('#TitlePrew').val(title);
            $('#ContentPrew').val(content);

            editor.setData(content);

            $('.ck-content p:first-child').html(content)
        });

    });    
}

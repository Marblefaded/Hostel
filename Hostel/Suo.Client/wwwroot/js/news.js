var countList = 0;
var isLoading = false;

function getPosts() {
    if (isLoading) return;
    isLoading = true;

    $.ajax({
        url: '/Post/GetListPostView',
        type: 'POST',
        data: { step: countList },
        success: function(result) {
            let parentBlock = document.querySelector('.news-list');

            result.forEach((x) => {
                parentBlock.innerHTML += x;
            });

            countList += 20;
            isLoading = false;
        },
        error: function() {
            isLoading = false;
        }
    });
}

var mainBlock = document.getElementById('main');
mainBlock.addEventListener('scroll', function() {
    var distanceFromBottom = mainBlock.scrollHeight - mainBlock.scrollTop - mainBlock.clientHeight;
    if (distanceFromBottom <= 300) {
        getPosts();
    }
});

getPosts();
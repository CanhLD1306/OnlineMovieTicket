$(document).ready(function () {
    loadBanners();
    loadMoviesCommingSoon();
    loadMoviesNowShowing();
});

function showTrailer(youtubeUrl) {
    const embedUrl = youtubeUrl.replace("watch?v=", "embed/");
    $('#trailerIframe').attr('src', embedUrl + '?autoplay=1');
    
    var trailerModal = new bootstrap.Modal(document.getElementById('trailerModal'));
    trailerModal.show();
}

function stopTrailer() {
    $('#trailerIframe').attr('src', '');
}

function loadBanners() {
    $.ajax({
        url: urlGetBanners,
        type: 'GET',
        success: function (data) {
            $('#banner-placeholder').html(data);
        },
        error: function (error) {
            console.error("Error loading banners:", error);
        }
    });
}

function loadMoviesCommingSoon() {
    $.ajax({
        url: urlGetMoviesCommingSoon,
        type: 'GET',
        success: function (data) {
            $('#movies-commingsoon-placeholder').html(data);
        },
        error: function (error) {
            console.error("Error loading banners:", error);
        }
    });
}

function loadMoviesNowShowing() {
    $.ajax({
        url: urlGetMoviesNowShowing,
        type: 'GET',
        success: function (data) {
            $('#movies-nowshowing-placeholder').html(data);
        },
        error: function (error) {
            console.error("Error loading banners:", error);
        }
    });
}

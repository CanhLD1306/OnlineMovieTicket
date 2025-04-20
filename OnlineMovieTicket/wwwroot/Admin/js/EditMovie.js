$(document).ready(function () {
    previewTrailer();

    $(document).on('submit', '#editMovieForm', function (e) {
        e.preventDefault();

        if (!$('#editMovieForm').valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var form = $('#editMovieForm')[0];
        var formData = new FormData(form);

        $.ajax({
            type: 'POST',
            url: urlEditMovie,
            data: formData,
            contentType: false,
            processData: false,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    setTimeout(function () {
                        window.location.href = urlIndex;
                    }, 2000);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });
});

function previewImage(event, previewId, messageSpanId) {
    const preview = document.getElementById(previewId);
    const messageSpan = document.getElementById(messageSpanId);
    const file = event.target.files[0];

    messageSpan.textContent = "";
    messageSpan.style.display = "none";

    if (file) {
        const validImageTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];

        if (!validImageTypes.includes(file.type)) {
            messageSpan.textContent = "Only image files (JPG, PNG, GIF, WEBP) are allowed";
            messageSpan.style.display = "block";
            event.target.value = "";
            preview.src = "#";
            preview.style.display = "none";
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = "block";
        };
        reader.readAsDataURL(file);
    } else {
        preview.src = "#";
        preview.style.display = "none";
    }
}


function previewTrailer() {
    const input = document.getElementById("trailerInput");
    const preview = document.getElementById("trailerPreview");
    const container = document.getElementById("trailerPreviewContainer");
    const url = input.value;

    // Regex bắt ID từ link YouTube
    const youtubeRegex = /(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/watch\?v=|youtu\.be\/)([^\s&]+)/;
    const match = url.match(youtubeRegex);

    if (match && match[1]) {
        const videoId = match[1];
        const embedUrl = `https://www.youtube.com/embed/${videoId}`;
        preview.src = embedUrl;
        container.style.display = "block";
    } else {
        preview.src = "";
        container.style.display = "none";
    }
}
let filterValue = "";

$(document).ready(function () {
    loadProfile();
    $('button[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        const targetId = $(e.target).data('bsTarget');
        localStorage.setItem('selectedTab', targetId);
        if (targetId === '#profile') {
            loadProfile();
        } else if (targetId === '#password') {
            loadChangePassword();
        } else if (targetId === '#tickets') {
            filterValue = "";
            $('input[name="ticketFilter"]').prop('checked', false);
            $('#allTickets').prop('checked', true);
            loadTickets(5, filterValue)
        }
    });

    $('input[name="ticketFilter"]').change(function () {
        filterValue = $(this).val();
        loadTickets(5, filterValue);
    });

    $(document).on('submit', '#profileForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var form = $('#profileForm')[0];
        var formData = new FormData(form);
        $.ajax({
            url: urlUploadProfile,
            type: 'POST',
            contentType: false,
            processData: false,
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    setTimeout(function () {
                        location.reload();
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

    $(document).on('submit', '#changePasswordForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlChangePassword,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    loadChangePassword();
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
function loadProfile (){
    $.ajax({
        url: urlGetProfile,
        type: 'GET',
        success: function (response) {
            $('#profileInformation').html(response);
            $.validator.unobtrusive.parse('#profileForm');
        },
        error: function (xhr, status, error) {
            toastr.error('There was an error processing your request: ' + error);
        }
    });
}

function loadChangePassword(){
    $.ajax({
        url: urlChangePassword,
        type: 'GET',
        success: function (response) {
            $('#changePassword').html(response);
            $.validator.unobtrusive.parse('#changePasswordForm');
            const inputPassword = document.getElementById("inputPassword");
            const reqLength = document.getElementById("req-length");
            const reqUppercase = document.getElementById("req-uppercase");
            const reqLowercase = document.getElementById("req-lowercase");
            const reqNumber = document.getElementById("req-number");
            const reqSpecial = document.getElementById("req-special");

            if (inputPassword) {
                inputPassword.addEventListener("input", function () {
                    const password = inputPassword.value;
                    reqLength.classList.toggle("text-success", password.length >= 6);
                    reqUppercase.classList.toggle("text-success", /[A-Z]/.test(password));
                    reqLowercase.classList.toggle("text-success", /[a-z]/.test(password));
                    reqNumber.classList.toggle("text-success", /\d/.test(password));
                    reqSpecial.classList.toggle("text-success", /[^A-Za-z0-9]/.test(password));
                });
            }
        },
        error: function (xhr, status, error) {
            toastr.error('There was an error processing your request: ' + error);
        }
    });
}

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

function loadTickets(maxRecord, isUpcoming) {
    $.ajax({
        url: urlGetTickets,
        type: 'GET',
        data: {maxRecord: maxRecord, isUpcoming: isUpcoming},
        success: function (response) {
            $('#ticketList').html(response);
        },
        error: function (xhr, status, error) {
            toastr.error('There was an error processing your request: ' + error);
        }
    });
}

function loadMoreTickets() {
    var totalCount = parseInt(document.getElementById('totalCount').value);
    var currentCount = parseInt(document.getElementById('currentCount').value);
    if(currentCount < totalCount){
        loadTickets(currentCount + 5,filterValue)
    }
}
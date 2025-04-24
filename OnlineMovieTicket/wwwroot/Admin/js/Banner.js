let sortBy = "CreatedAt";
let pageSize = 5;
let isDescending = true;

$(document).ready(function () {
    

    // Banner Table

    $('#bannersTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        paging: false,
        lengthMenu: false,
        ajax: {
            url: urlGetBanners,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    IsActive: $('#statusFilter').val(),
                    PageNumber: 1,
                    PageSize: pageSize,
                    SortBy: sortBy,
                    IsDescending: isDescending
                };
            }
        },  
        columns: [
            { "data": 'imageUrl',
                "render": function (data, type, row) {
                    return `<img src="${data}" alt="Banner Image" style="width: 250px; height: auto;">`;
                }
            },
            { "data": 'title' },
            { "data": 'isActive',
                className: "text-center",
                "render": function (data, type, row) {
                    const checked = data ? 'checked' : '';
                    return `
                        <div class="custom-control custom-switch">
                            <input type="checkbox" class="custom-control-input toggle-status" id="switch-${row.id}" data-id="${row.id}" ${checked}>
                            <label class="custom-control-label" for="switch-${row.id}"></label>
                        </div>`;    
                }
            },
            {
                "data": null,
                className: "text-center",
                "render": function (data, type, row) {
                    return `<a class='btn btn-sm btn-info btn-edit-banner' title='Edit' data-id='${row.id}'>
                                <i class='fas fa-edit'></i>
                            </a>
                            <a class='btn btn-sm btn-danger ml-1 btn-delete-banner' title='Delete' data-id='${row.id}'>
                                <i class='fas fa-trash'></i>
                            </a>`;
                }
            }
        ],
        language: {
            emptyTable: "No data available in table",
            infoFiltered: "",
        }
    });

    $('#searchTerm').on('input', function (e) {
        $('#bannersTable').DataTable().ajax.reload();
    });

    $('#statusFilter').on('change', function () {
        $('#bannersTable').DataTable().ajax.reload();
    });

    // Add New Banner

    $('#addBtn').click(function(e) {
        e.preventDefault();
        $.ajax({
            url: urlCreate,
            type: 'GET',
            success: function (response) {
                $('#bannerModal .modal-content').html(response);
                $.validator.unobtrusive.parse('#addBannerForm');
                $('#bannerModal').modal('show');
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#addBannerForm', function (e) {
        e.preventDefault();
        if (!$('#addBannerForm').valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var form = $('#addBannerForm')[0];
        var formData = new FormData(form);
        $.ajax({
            url: urlCreate,
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
                    $('#bannerModal').modal('hide');
                    $('#bannersTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Edit Banner

    $(document).on('click', '.btn-edit-banner', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlEdit,
            type: 'GET',
            data: { bannerId: id },           
            success: function (response) {
                if (response) {
                    $('#bannerModal .modal-content').html(response);
                    $.validator.unobtrusive.parse('#editBannerForm');
                    $('#bannerModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#editBannerForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var form = $('#editBannerForm')[0];
        var formData = new FormData(form);
        $.ajax({
            url: urlEdit,
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
                    $('#bannerModal').modal('hide');
                    $('#bannersTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Change Status

    $(document).on('focusin', '.toggle-status', function () {
        const currentChecked = $(this).prop('checked');
        $(this).data('prev', currentChecked);
    });

    $(document).on('change', '.toggle-status', function () {
        const $checkbox = $(this);
        const bannerId = $checkbox.data('id');
        $.ajax({
            url: urlChangeStatus,
            type: 'POST',
            data: {bannerId : bannerId},
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#bannersTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                    $checkbox.prop('checked', $checkbox.data('prev'));
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
                $checkbox.prop('checked', $checkbox.data('prev'));
            }
        });
    });

    // Delete Banner

    $(document).on('click', '.btn-delete-banner', function (e) {
        e.preventDefault();
        var bannerId = $(this).data('id');
        $.ajax({
            url: urlDelete,
            type: 'GET',
            data: { bannerId: bannerId },           
            success: function (response) {
                if (response) {
                    $('#bannerModal .modal-content').html(response);
                    $('#bannerModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#deleteBannerForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlDelete,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#bannerModal').modal('hide');
                    $('#bannersTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Anothetr
});

function previewImage(event) {
    const input = event.target;
    const preview = document.getElementById('imagePreview');

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
        };

        reader.readAsDataURL(input.files[0]);
    } else {
        preview.src = '#';
        preview.style.display = 'none';
    }
}
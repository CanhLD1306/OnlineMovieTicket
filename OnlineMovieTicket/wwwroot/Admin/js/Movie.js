$(document).ready(function () {
    let sortBy = "CreatedAt";
    let startDate = "";
    let endDate = "";
    let isDescending = true;
    let isAdjustingPage = false;

    $('#moviesTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetMovies,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    StartDate: startDate,
                    EndDate: endDate,
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                    SortBy: sortBy,
                    IsDescending: isDescending
                };
            }
        },  
        columns: [
            { "data": 'title'},
            { "data": 'duration', className: "text-center"},
            { "data": 'price', className: "text-center"},
            { "data": 'releaseDate', 
                className: "text-center",
                "render": function (data, type, row) {
                    const date = new Date(data);
                    return date.toLocaleDateString('vi-VN');
                }
            },
            {
                "data": null,
                className: "text-center",
                "render": function (data, type, row) {
                    return `<a class='btn btn-sm btn-info btn-edit-movie' title='Edit' data-id='${row.id}'>
                                <i class='fas fa-edit'></i>
                            </a>
                            <a class='btn btn-sm btn-danger ml-1 btn-delete-movie' title='Delete' data-id='${row.id}'>
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

    $('#searchTerm').on('keyup', function () {
        $('#moviesTable').DataTable().ajax.reload(null, false);
    });
    $('#startDate').on('change', function () {
        startDate = $(this).val();
        $('#endDate').attr('min', startDate);
        $('#moviesTable').DataTable().ajax.reload(null, false);
    });
    $('#endDate').on('change', function () {
        endDate = $(this).val();
        $('#startDate').attr('max', endDate);
        $('#moviesTable').DataTable().ajax.reload(null, false);
    });


    // Delete Movie

    $(document).on('click', '.btn-delete-movie', function (e) {
        e.preventDefault();
        var movieId = $(this).data('id');
        $.ajax({
            url: urlDelete,
            type: 'GET',
            data: { movieId: movieId },           
            success: function (response) {
                if (response) {
                    $('#movieModal .modal-content').html(response);
                    $('#movieModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#deleteMovieForm', function (e) {
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
                    $('#movieModal').modal('hide');
                    $('#moviesTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Edit
    $(document).on('click', '.btn-edit-movie', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlEdit,
            type: 'GET',
            data: { movieId: id },           
            success: function (response) {
                if (response) {
                    window.location.href = `${urlEdit}?movieId=${id}`    
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
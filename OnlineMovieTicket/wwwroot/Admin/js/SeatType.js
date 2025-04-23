$(document).ready(function () {
    let sortBy = "CreatedAt";
    let isDescending = true;
    let isAdjustingPage = false;
    const searchTerm = $('#searchTerm');

    // List, search, filter and sort SeatType in table

    $('#seatTypesTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetSeatType,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: searchTerm.val(),
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                    SortBy: sortBy,
                    IsDescending: isDescending
                };
            }
        },
        columns: [
            { "data": 'name' },
            { "data": 'priceMultiplier', className: "text-center" },
            { "data": 'color',
                className: "text-center",
                "render": function (data, type, row) {
                    return `<span style="display:inline-block;width:20px;height:20px;background-color:${data};"></span>`;
                }
            },
            {
                "data": null,
                className: "text-center",
                "render": function (data, type, row) {
                    return `<a class='btn btn-sm btn-info btn-edit-seatType' title='Edit' data-id='${row.id}'>
                                <i class='fas fa-edit'></i>
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
        $('#seatTypesTable').DataTable().ajax.reload();
    });

    // Edit SeatType

    $(document).on('click', '.btn-edit-seatType', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlEdit,
            type: 'GET',
            data: { seatTypeId: id },           
            success: function (response) {
                if (response) {
                    $('#seatTypeModal .modal-content').html(response);
                    $.validator.unobtrusive.parse('#editSeatTypeForm');
                    $('#seatTypeModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#editSeatTypeForm', function (e) {
        e.preventDefault();
        if (!$(this).valid()) {
            return;
        }
        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlEdit,
            type: 'POST',
            data: formData,
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#seatTypeModal').modal('hide');
                    $('#seatTypesTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Another, Funtion

    $(document).on('hidden.bs.modal', '.modal', function () {
        $('#seatTypeModal .modal-content').html('');
    });
});

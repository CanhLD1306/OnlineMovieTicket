$(document).ready(function () {
    let sortBy = "CreatedAt";
    let startDate = "";
    let endDate = "";
    let isDescending = true;
    let isAdjustingPage = false;

    loadCountries($('#countryFilter'));

    $('#showtimesTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetShowtimes,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    CinemaId: $('#cinemaFilter').val(),
                    RoomId: $('#roomFilter').val(),
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
            { "data": 'movieTitle' },
            { "data": 'roomName' },
            { "data": 'cinemaName' },
            {
                "data": 'startTime',
                "className": "text-center",
                "render": function (data, type, row) {
                    const date = new Date(data);
                    return date.toLocaleDateString('vi-VN');
                }
            },
            {
                "data": 'startTime',
                "className": "text-center",
                "render": function (data, type, row) {
                    const date = new Date(data);
                    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }); // HH:mm
                }
            },
            {
                "data": 'endTime',
                "className": "text-center",
                "render": function (data, type, row) {
                    const date = new Date(data);
                    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }); // HH:mm
                }
            },
            {
                "data": null,
                className: "text-center",
                "render": function (data, type, row) {
                    return `<a class='btn btn-sm btn-info btn-edit-showtime' title='Edit' data-id='${row.id}'>
                                <i class='fas fa-edit'></i>
                            </a>
                            <a class='btn btn-sm btn-danger ml-1 btn-delete-showtime' title='Delete' data-id='${row.id}'>
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

    //Search filter
    $('#searchTerm').on('keyup', function () {
        $('#showtimesTable').DataTable().ajax.reload();
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

    $('#applyFilterBtn').on('click', function () {
        startDate = $('#startDate').val();
        endDate = $('#endDate').val();
        
        $('#showtimesTable').DataTable().ajax.reload();
        $('#filterModal').modal('hide');
    });

    $('#resetBtn').on('click', function () {
        $('#countryFilter').val('');
        $('#cityFilter').val('').prop('disabled', true);
        $('#cinemaFilter').val('').prop('disabled', true);
        $('#roomFilter').val('').prop('disabled', true);
        $('#startDate').val('');
        $('#endDate').val('');
        startDate = '';
        endDate = '';
        $('#showtimesTable').DataTable().ajax.reload();
    });

    $('#countryFilter').on('change', function () {
        const countryId = $(this).val();
        loadCities($('#cityFilter'), countryId);
        $('#cinemaFilter').val('').prop('disabled', true);
        $('#roomFilter').val('').prop('disabled', true);
    });
    $('#cityFilter').on('change', function () {
        const cityId = $(this).val();
        loadCinemas($('#cinemaFilter'), cityId);
        $('#roomFilter').val('').prop('disabled', true);
    });

    $('#cinemaFilter').on('change', function () {
        const cinemaId = $(this).val();
        loadRooms($('#roomFilter'), cinemaId);
    });

    // Delete showtime

    $(document).on('click', '.btn-delete-showtime', function (e) {
        e.preventDefault();
        var showtimeId = $(this).data('id');
        $.ajax({
            url: urlDelete,
            type: 'GET',
            data: { showtimeId: showtimeId },           
            success: function (response) {
                if (response) {
                    $('#showtimeModal .modal-content').html(response);
                    $('#showtimeModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#deleteShowtimeForm', function (e) {
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
                    $('#showtimeModal').modal('hide');
                    $('#showtimesTable').DataTable().ajax.reload(null, false);
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

function loadCountries(selectElement) {
    $.ajax({
        url: urlGetAllCountries,
        type: 'GET',
        success: function (data) {
            selectElement.val("");
            selectElement.find('option').not('[value=""]').remove();
            $.each(data, function (i, country) {
                selectElement.append($('<option>', {
                    value: country.id,
                    text: country.name,
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load countries.");
        }
    });
}
function loadCities(selectElement, countryId = null) {
    $.ajax({
        url: urlGetCities,
        type: 'GET',
        data: {countryId: countryId},
        success: function (data) {
            selectElement.val("");
            selectElement.find('option').not('[value=""]').remove();
            if(!countryId){
                selectElement.prop('disabled', true);
            }else{
                selectElement.prop('disabled', false);
            }
            $.each(data, function (i, city) {
                selectElement.append($('<option>', {
                    value: city.id,
                    text: city.name,
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load Cities.");
        }
    });
}
function loadCinemas(selectElement, cityId = null) {
    $.ajax({
        url: urlGetCinemas,
        type: 'GET',
        data: {cityId: cityId},
        success: function (data) {
            selectElement.val("");
            selectElement.find('option').not('[value=""]').remove();
            if(!cityId){
                selectElement.prop('disabled', true);
                return;
            }else{
                selectElement.prop('disabled', false);
            }
            $.each(data, function (i, cinema) {
                selectElement.append($('<option>', {
                    value: cinema.id,
                    text: cinema.name,
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load Cinema.");
        }
    });
}
function loadRooms(selectElement, cinemaId = null) {
    $.ajax({
        url: urlGetRooms,
        type: 'GET',
        data: {cinemaId: cinemaId},
        success: function (data) {
            selectElement.val("");
            selectElement.find('option').not('[value=""]').remove();
            if(!cinemaId){
                selectElement.prop('disabled', true);
                return;
            }else{
                selectElement.prop('disabled', false);
            }
            $.each(data, function (i, room) {
                selectElement.append($('<option>', {
                    value: room.id,
                    text: room.name,
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load Rooms.");
        }
    });
}
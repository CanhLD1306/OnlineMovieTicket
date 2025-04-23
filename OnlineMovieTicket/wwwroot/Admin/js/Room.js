$(document).ready(function () {
    let sortBy = "CreatedAt";
    let isDescending = true;
    let isAdjustingPage = false;
    loadCountries($('#countryFilter'));

    $('#roomsTable').DataTable({
        processing: true,
        serverSide: true,
        searching: false,
        ordering: false,
        lengthMenu: [5,10,25,50],
        ajax: {
            url: urlGetRooms,
            type: 'POST',
            data: function (d) {
                return {
                    Draw: d.draw,
                    SearchTerm: $('#searchTerm').val(),
                    CountryId: $('#countryFilter').val(),
                    CityId: $('#cityFilter').val(),
                    CinemaId: $('#cinemaFilter').val(),
                    IsAvailable: $('#statusFilter').val(),
                    PageNumber: (d.start / d.length) + 1,
                    PageSize: d.length,
                    SortBy: sortBy,
                    IsDescending: isDescending
                };
            }
        },  
        columns: [
            { "data": 'name' },
            { "data": 'cityName' },
            { "data": 'cinemaName' },
            { "data": 'capacity' , className: "text-center"},
            { "data": 'isAvailable',
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
                    return `<a class='btn btn-sm btn-info btn-edit-room' title='Edit' data-id='${row.id}'>
                                <i class='fas fa-edit'></i>
                            </a>
                            <a class='btn btn-sm btn-danger ml-1 btn-delete-room' title='Delete' data-id='${row.id}'>
                                <i class='fas fa-trash'></i>
                            </a>`;
                }
            }
        ],
        language: {
            emptyTable: "No data available in table",
            infoFiltered: "",
            paginate: {
                previous: '&lt;',
                next: '&gt;',
            }
        }
    });

    $('#searchTerm').on('input', function (e) {
        $('#roomsTable').DataTable().ajax.reload();
    });

    $('#countryFilter').on('change', function () {
        const countryId = $(this).val();
        $('#cityFilter').val("");
        $('#cinemaFilter').val("");
        loadCities($('#cityFilter'), countryId);
        loadCinemas($('#cinemaFilter'));
        $('#roomsTable').DataTable().ajax.reload();
    });

    $('#cityFilter').on('change', function () {
        const cityId = $(this).val();
        $('#cinemaFilter').val("");
        loadCinemas($('#cinemaFilter'), cityId);
        $('#roomsTable').DataTable().ajax.reload();
    });

    $('#cinemaFilter').on('change', function () {
        $('#roomsTable').DataTable().ajax.reload();
    });

    $('#statusFilter').on('change', function () {
        $('#roomsTable').DataTable().ajax.reload();
    });

    $(document).on('click', '.sort-header', function (e) {
        const clickedSort = $(this).data('sort');
        toggleSort(clickedSort);
    });

    // Edit Room

    $(document).on('click', '.btn-edit-room', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlEdit,
            type: 'GET',
            data: { roomId: id },           
            success: function (response) {
                if (response) {
                    window.location.href = `${urlEdit}?roomId=${id}`    
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Delete room
    $(document).on('click', '.btn-delete-room', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        $.ajax({
            url: urlDelete,
            type: 'GET',
            data: { id: id },           
            success: function (response) {
                if (response) {
                    $('#roomModal .modal-content').html(response);
                    $('#roomModal').modal('show');
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    $(document).on('submit', '#deleteRoomForm', function (e) {
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
                    $('#roomModal').modal('hide');
                    $('#roomsTable').DataTable().ajax.reload(null, false);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error('There was an error processing your request: ' + error);
            }
        });
    });

    // Change status

    $(document).on('change', '.toggle-status', function () {
        const $checkbox = $(this);
        const cinemaId = $checkbox.data('id');
        $.ajax({
            url: urlChangeStatus,
            type: 'POST',
            data: {id : cinemaId},
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#roomsTable').DataTable().ajax.reload(null, false);
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
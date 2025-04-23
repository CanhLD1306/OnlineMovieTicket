$(document).ready(function () {
    let seatDataList= [];
    loadCountries($('#addCountrySelected'));
    loadMovies($('#addMovieSelected'));

    $('#addCountrySelected').on('change', function () {
        const countryId = $(this).val();
        loadCities($('#addCitySelected'), countryId);
        $('#addCinemaSelected').val('').prop('disabled', true);
        $('#addRoomSelected').val('').prop('disabled', true);
    });

    $('#addCitySelected').on('change', function () {
        const cityId = $(this).val();
        loadCinemas($('#addCinemaSelected'), cityId);
        $('#addRoomSelected').val('').prop('disabled', true);
    });

    $('#addCinemaSelected').on('change', function () {
        const cinemaId = $(this).val();
        loadRooms($('#addRoomSelected'), cinemaId);
    });

    $('#startTime').on('change', function () {
        const startTime = $(this).val();
        const duration = parseInt($('#addMovieSelected option:selected').data('duration')) || 0;
        if (startTime) {
            const start = new Date(startTime);
            const end = new Date(start.getTime() + (duration + 15) * 60000);
            $('#endTime').val(formatDateTimeLocal(end));
        } else {
            $('#endTime').val('');
        }
    });

    $('#addRoomSelected').on('change', function () {
        const roomId = $(this).val();
        if (roomId) {
            $.ajax({
                url: urlGetRoomWithSeats,
                type: 'GET',
                data: { roomId: roomId },
                success: function (response) {
                    if(response.success){
                        console.log("AJAX response:", response);
                        seatDataList = response.data.seats;
                        generateSeats(seatDataList);
                    }else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Failed to load seats.");
                }
            });
        } else {
            seatDataList = [];
            $('#seatsContainer').empty();
        }
    });

    $('#addMovieSelected').on('change', function () {
        const selectedOption = $(this).find('option:selected');
        const movieId = $(this).val();
        const releaseDate = selectedOption.data('releasedate');
        const duration = parseInt(selectedOption.data('duration')) || 0;

        if (movieId) {
            $('input[name="StartTime"]').prop('disabled', false);
            $('input[name="EndTime"]').prop('disabled', false);

            if (releaseDate) {
                const start = new Date(releaseDate);
                const end = new Date(start.getTime() + (duration + 15) * 60000);

                $('input[name="StartTime"]').val(formatDateTimeLocal(start));
                $('input[name="EndTime"]').val(formatDateTimeLocal(end));
            } else {
                $('input[name="StartTime"]').val('');
                $('input[name="EndTime"]').val('');
            }
        } else {
            $('input[name="StartTime"]').prop('disabled', true).val('');
            $('input[name="EndTime"]').prop('disabled', true).val('');
        }
    });

    $('#addShowtimeForm').on('submit', function (e) {
        e.preventDefault();
        const formData = $(this).serializeArray();
        $.ajax({
            url: urlCreateShowtime,
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    window.location.href = urlIndex;
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error("Failed to create showtime.");
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
function loadMovies(selectElement) {
    $.ajax({
        url: urlGetAllMovies,
        type: 'GET',
        success: function (data) {
            selectElement.val("");
            selectElement.find('option').not('[value=""]').remove();
            $.each(data, function (i, movie) {
                selectElement.append($('<option>', {
                    value: movie.id,
                    text: movie.title,
                    'data-duration': movie.duration,
                    'data-releasedate': movie.releaseDate,
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load Rooms.");
        }
    });
}
function formatDateTimeLocal(date) {
    const offset = date.getTimezoneOffset();
    const local = new Date(date.getTime() - offset * 60000);
    return local.toISOString().slice(0, 16);
}

function generateSeats(seatDataList) {
    if (!Array.isArray(seatDataList) || seatDataList.length === 0) return;
    const seatsContainer = $("#seatsContainer");
    seatsContainer.empty();

    const maxRow = Math.max(...seatDataList.map(s => s.rowIndex));
    const maxCol = Math.max(...seatDataList.map(s => s.columnIndex));

    seatsContainer.css("grid-template-columns", `repeat(${maxCol}, 0.1fr)`);
    const screenDiv = $(`<div class="screen" style="grid-column: span ${maxCol};">SCREEN</div>`);
    seatsContainer.append(screenDiv);

    for (let r = 0; r <= maxRow; r++) {
        const rowLetter = String.fromCharCode(65 + r);
        for (let c = 1; c <= maxCol; c++) {
            const seat = seatDataList.find(s => s.rowIndex === r && s.columnIndex === c)
            const seatCode = rowLetter + c;
            const seatColor = seat.color;
            const seatDiv = $(`
                <div class="seat btn btn-outline-secondary" 
                    data-seat="${seatCode}" 
                    data-row="${r}" 
                    data-col="${c}"
                    style="background-color: ${seatColor}; cursor: default;">
                    ${seatCode}
                </div>
            `);

            seatsContainer.append(seatDiv);
        }
    }
}
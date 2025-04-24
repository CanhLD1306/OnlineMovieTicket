$(document).ready(function () {
    loadCinemas($('#editCinemaSelected'));
    loadMovies($('#editMovieSelected'));
    generateSeats(seatDataList);

    $(document).on('submit', '#editShowtimeForm', function (e) {
        e.preventDefault();
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
    $('#startTime').on('change', function () {
        const startTime = $(this).val();
        const duration = parseInt($('#editMovieSelected option:selected').data('duration')) || 0;
        if (startTime) {
            const start = new Date(startTime);
            const end = new Date(start.getTime() + (duration + 15) * 60000);
            $('#endTime').val(formatDateTimeLocal(end));
        } else {
            $('#endTime').val('');
        }
    });
});

function formatDateTimeLocal(date) {
    const offset = date.getTimezoneOffset();
    const local = new Date(date.getTime() - offset * 60000);
    return local.toISOString().slice(0, 16);
}

function loadCinemas(selectElement) {
    const cityId = $('input[name="Showtime.CityId"]').val();
    $.ajax({
        url: urlGetCinemas,
        type: 'GET',
        data: {cityId: cityId},
        success: function (data) {
            const cinemaId = selectElement.data('selected');
            $.each(data, function (i, cinema) {
                selectElement.append($('<option>', {
                    value: cinema.id,
                    text: cinema.name,
                    selected: cinema.id === cinemaId
                }));
            });
            loadRooms($('#editRoomSelected'), cinemaId);
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
            const roomId = selectElement.data('selected');
            $.each(data, function (i, room) {
                selectElement.append($('<option>', {
                    value: room.id,
                    text: room.name,
                    selected: room.id === roomId
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load Rooms.");
        }
    });
}

function loadMovies(selectElement, movieId = null) {
    $.ajax({
        url: urlGetAllMovies,
        type: 'GET',
        data: {movieId: movieId},
        success: function (data) {
            const movieId = selectElement.data('selected');
            $.each(data, function (i, movie) {
                selectElement.append($('<option>', {
                    value: movie.id,
                    text: movie.title,
                    'data-duration': movie.duration,
                    'data-releasedate': movie.releaseDate,
                    selected: movie.id === movieId
                }));
            });
        },
        error: function () {
            toastr.error("Failed to load Movies.");
        }
    });
}

function generateSeats(seatDataList) {
    console.log("Generating seats..." + JSON.stringify(seatDataList, null, 2));
    if (!Array.isArray(seatDataList) || seatDataList.length === 0) return;
    const seatsContainer = $("#seatsContainer");
    seatsContainer.empty();

    const maxRow = Math.max(...seatDataList.map(s => s.RowIndex));
    const maxCol = Math.max(...seatDataList.map(s => s.ColumnIndex));

    seatsContainer.css("grid-template-columns", `repeat(${maxCol}, 0.1fr)`);
    const screenDiv = $(`<div class="screen" style="grid-column: span ${maxCol};">SCREEN</div>`);
    seatsContainer.append(screenDiv);

    for (let r = 0; r <= maxRow; r++) {
        const rowLetter = String.fromCharCode(65 + r);
        for (let c = 1; c <= maxCol; c++) {
            const seat = seatDataList.find(s => s.RowIndex === r && s.ColumnIndex === c)
            const seatCode = rowLetter + c;
            const seatColor = seat.IsBooked === true ? '#808080' : seat.color;
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
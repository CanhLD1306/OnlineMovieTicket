let defaultSeatType = null;
let selectedSeats = [];
let seatDataList = [];
let seatTypeList = [];
$(document).ready(function () {
    loadCountries($('#addCountrySelected'));
    loadSeatTypes($('#addSeatTypeSelect'));

    $("#Room_Row").on("input", function () {
        var minValue = $(this).attr("min");
        var maxValue = $(this).attr("max");
        applyLimitRealtime(this, maxValue, minValue);
    });

    $("#Room_Column").on("input", function () {
        var minValue = $(this).attr("min");
        var maxValue = $(this).attr("max");
        applyLimitRealtime(this, maxValue, minValue);
    });

    $("#generateSeats").click(function () {
        generateSeats();
    });

    $('#addCountrySelected').on('change', function () {
        const countryId = $(this).val();
        loadCities($('#addCitySelected'), countryId);
        loadCinemas($('#addCinemaSelected'));
    });
    $('#addCitySelected').on('change', function () {
        const cityId = $(this).val();
        loadCinemas($('#addCinemaSelected'), cityId);
    });

    $("#applySeatTypeBtn").on("click", function () {
        const selectedSeatTypeId = $('#addSeatTypeSelect').val();
        const selectedSeatType = seatTypeList.find(st => st.id == selectedSeatTypeId);
        if (!selectedSeatTypeId) {
            toastr.warning("Please select a Seat Type.");
            return;
        }
        selectedSeats.forEach(seatCode => {
            const seatDiv = $(`[data-seat='${seatCode}']`);
            seatDiv.css("background-color", selectedSeatType.color);
            const seatData = seatDataList.find(s => 
                String.fromCharCode(65 + s.RowIndex) + s.ColumnIndex === seatCode
            );

            if (seatData) {
                seatData.SeatTypeId = selectedSeatType.id;
                seatData.Color = selectedSeatType.color;
            }
        });
        selectedSeats = [];
        $("#seatTypeGroup").addClass("d-none");
        $('#addSeatTypeSelect').val("")
        $(".seat").removeClass("selected");
        $("#SeatsJson").val(JSON.stringify(seatDataList));
    });

    $(document).on('submit', '#addRoomWithSeatsForm', function (e) {
        e.preventDefault();
        const currentRow = parseInt($("#Room_Row").val());
        const currentCol = parseInt($("#Room_Column").val());
        const maxRow = Math.max(...seatDataList.map(s => s.RowIndex)) + 1;
        const maxCol = Math.max(...seatDataList.map(s => s.ColumnIndex));

        if (!$('#addRoomWithSeatsForm').valid()) {
            return;
        }

        if (seatDataList.length === 0) {
            toastr.warning("Please click 'Generate Seats' to create the seat layout.");
            return;
        }

        if (currentRow !== maxRow || currentCol !== maxCol) {
            toastr.warning("The current number of rows and columns does not match the generated seat layout. Please click 'Generate Seats' again.");
            return;
        }

        var token = $('input[name="__RequestVerificationToken"]').val();
        var formData = $(this).serialize();
        $.ajax({
            url: urlCreate,
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
});

// Functions

function applyLimitRealtime(inputElement, maxValue, minValue) {
    let val = parseInt(inputElement.value);

    if (isNaN(val) || val <= 0) {
        inputElement.value = '';
        return;
    }

    if (val > maxValue) {
        inputElement.value = maxValue;
    }

    if(val < minValue){
        inputElement.value = minValue;
    }
}
function generateSeats() {
    const row = parseInt($("#Room_Row").val());
    const col = parseInt($("#Room_Column").val());

    let isValid = true;

    $("span[data-valmsg-for='Room.Row']").text('');
    $("span[data-valmsg-for='Room.Column']").text('');

    if (!row || row < 5 || row > 26) {
        $("span[data-valmsg-for='Room.Row']").text("Please enter Row and must be between 5-26");
        isValid = false;
    }

    if (!col || col < 5 || col > 20) {
        $("span[data-valmsg-for='Room.Column']").text("Please enter Column and must be between 5-20");
        isValid = false;
    }

    if (!isValid) return;

    const seatsContainer = $("#seatsContainer");
    seatsContainer.empty();
    seatsContainer.css("grid-template-columns", `repeat(${col}, 0.1fr)`);

    const seatMap = {};
    seatDataList.forEach(seat => {
        seatMap[`${seat.RowIndex}-${seat.ColumnIndex}`] = seat;
    });
    const updatedList = [];

    const screenDiv = $(`<div class="screen" style="grid-column: span ${col};">SCREEN</div>`);
    seatsContainer.append(screenDiv);

    for (let r = 0; r < row; r++) {
        const rowLetter = String.fromCharCode(65 + r);
        for (let c = 1; c <= col; c++) {
            const seatCode = rowLetter + c;
            const seatKey = `${r}-${c}`;
            let seat = seatMap[seatKey];
            
            if (!seat) {
                seat = {
                    Id: 0,
                    RowIndex: r,
                    ColumnIndex: c,
                    SeatTypeId: defaultSeatType.id,
                    Color: defaultSeatType.color,
                    IsDeleted: false
                };
            }

            const seatDiv = $(`
                <div class="seat btn btn-outline-secondary" 
                    data-seat="${seatCode}" 
                    data-row="${r}" 
                    data-col="${c}"
                    style="background-color: ${seat.Color};">
                    ${seatCode}
                </div>
            `);

            seatsContainer.append(seatDiv);
            updatedList.push(seat);

            seatDiv.on("click", function () {
                const seatCode = $(this).data("seat");

                if (selectedSeats.includes(seatCode)) {
                    selectedSeats = selectedSeats.filter(s => s !== seatCode);
                    $(this).removeClass("selected");
                } else {
                    selectedSeats.push(seatCode);
                    $(this).addClass("selected");
                }

                if (selectedSeats.length > 0) {
                    $("#seatTypeGroup").removeClass("d-none");
                } else {
                    $("#seatTypeGroup").addClass("d-none");
                }
            });
        }
    }
    seatDataList = updatedList;
    selectedSeats = [];
    $(".seat").removeClass("selected border-danger");
    $("#seatTypeGroup").addClass("d-none");

    console.log("seatDataList:", seatDataList);
    $("#SeatsJson").val(JSON.stringify(seatDataList));
}

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
                loadCinemas($('#addCinemaSelected'));
                return;
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

function loadSeatTypes(selectElement) {
    $.ajax({
        url: urlGetAllSeatType,
        type: 'GET',
        success: function (data) {
            selectElement.val("");
            $.each(data, function (i, seatType) {
                selectElement.append($('<option>', {
                    value: seatType.id,
                    text: seatType.name,
                }));
                seatTypeList = data;
            });

            if(!defaultSeatType){
                defaultSeatType = data.find(item => item.name.toLowerCase() === "standard");
            }

            generateSeats();
        },
        error: function () {
            toastr.error("Failed to load SeatType.");
        }
    });
}
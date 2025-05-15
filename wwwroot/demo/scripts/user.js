$(document).ready(async function () {

    await signIn();

    $("#get-spaces").on("submit", function (e) {
        e.preventDefault();
        getSpaces();
    });

    $("body").on("click", "#res-button", makeReservation);

    $("body").on("click", ".cancel-button", function () {
        var id = $(this).attr("data-id");
        cancelReservation(id);
    });

    getMyReservations();
});

function getSpaces() {
    $('#available').empty();
    $.ajax({
        url: `https://localhost:7036/api/Spaces/available?from=${$('#begins-at').val()}Z&till=${$('#ends-at').val()}Z`,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            $('#available').append(`<span>Obsazeno ${response.occupiedCount}/${response.totalCount}</span>`);
            if (response.available) {
                $('#available').append(`<button id="res-button">Rezervovat</span>`);
            }
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}

function makeReservation() {
    $("#make-reservation-result").empty();
    $.ajax({
        type: "POST",
        url: 'https://localhost:7036/api/Reservations',
        data: JSON.stringify({
            beginsAt: $('#begins-at').val() + "Z",
            endsAt: $('#ends-at').val() + "Z",
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            displayReservation(data);
            $('#make-reservation-result').append(`<p>Vytvořen požadavek na rezervaci číslo ${data.id}.</p>`);
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}

function getMyReservations() {
    $.ajax({
        url: 'https://localhost:7036/api/Reservations/my',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            for (var row of response) {
                displayReservation(row);
            }

        },
        error: function (jqXHR, textStatus) {
            console.error(textStatus, jqXHR);
        }
    });
}

function cancelReservation(id) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7036/api/Reservations/${id}`,
        success: function () {
            $(`#res-${id}`).find(".state").text('zrušeno');
        },
        error: function (jqXHR) {
            alert(jqXHR.responseText)
        }
    });
}

function displayReservation(reservation) {
    $('#my').append(`<tr id="res-${reservation.id}">
            <td>${reservation.id}</td>
            <td>${reservation.beginsAt}</td>
            <td>${reservation.endsAt}</td>
            <td>${reservation.spaceNumber}</td>
            <td>${reservation.createdAt}</td>
            <td class="state">${reservation.state}</td>
        </tr>`);
    if (reservation.isActive) {
        $(`#res-${reservation.id}`).append(`<td><button data-id="${reservation.id}" class="cancel-button">Zrušit</button></td>`);
    }
}
$(document).ready(async function () {
    await signIn();

    $("#add-spaces").on("submit", function (e) {
        e.preventDefault();
        AddSpaces();
    });

    $("body").on("click", ".delete-space-button", function () {
        var spaceNumber = $(this).attr("data-space-number");
        deleteSpace(spaceNumber);
    });

    $("body").on("click", ".cancel-button", function () {
        var id = $(this).attr("data-id");
        cancelReservation(id);
    });

    $("body").on("click", ".confirm-button", function () {
        var id = $(this).attr("data-id");
        confirmReservation(id);
    });

    $("body").on("click", ".confirm-all-button", function () {
        confirmAll();
    });

    $("body").on("click", ".unblock-button", function () {
        var id = $(this).attr("data-id");
        unblock(id);
    });

    $("body").on("click", ".detail-button", function () {
        var spaceNumber = $(this).attr("data-space-number");
        getSpaceDetail(spaceNumber);
    });

    $("body").on("click", ".block-dialog-button", function () {
        var spaceNumber = $(this).attr("data-space-number");
        blockSpaceDialog(spaceNumber);
    });

    $("body").on("submit", ".block-form", function (e) {
        e.preventDefault();
        var spaceNumber = $(this).attr("data-space-number");
        blockSpace(spaceNumber);
    });

    getSpaces();
    getRequests();
    getBlockings();
});

function AddSpaces() {
    $("#add-op-result").empty();
    $.ajax({
        type: "POST",
        url: 'https://localhost:7036/api/Spaces',
        data: JSON.stringify({
            count: $('#count').val()
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            displaySpace(data);
            $('#add-spaces-result').append(`<p>Přidána místa (${data.lenght}).</p>`);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}
function getSpaces() {
    $.ajax({
        url: 'https://localhost:7036/api/Spaces',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            for (var row of response) {
                displaySpace(row);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}


function getRequests() {
    $.ajax({
        url: 'https://localhost:7036/api/Reservations/requests',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            for (var row of response) {
                $('#requests').append(`<tr id=res-${row.id}>
                <td>${row.id}</td>
                <td>${row.beginsAt}</td>
                <td>${row.endsAt}</td>
                <td>${row.spaceNumber}</td>
                <td>${row.issuedAt}</td>
                <td>${row.userId}</td>
                <td><button class="confirm-button" data-id=${row.id}>Potvrdit</button></td>
                <td><button class="cancel-button" data-id=${row.id}>Zrušit</button></td>
                </tr>`);
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function getBlockings() {
    $.ajax({
        url: 'https://localhost:7036/api/Blockings',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            for (var row of response) {
                displayBlocking(row);
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function getSpaceDetail(id) {
    $('.calendar').remove();
    $.ajax({
        url: `https://localhost:7036/api/Spaces/${id}`,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            $(`#sp-${id}`).after(
                `<tr class="calendar">
                    <td colspan=5>
                        <table class="calendar-table">
                            <tr>
                                <th>Číslo rezervace/blokace</th>
                                <th>Od</th>
                                <th>Do</th>
                                <th>Zrušit</th>
                            </tr>
                        </table>
                    <td>
                </tr>`
            )
            for (var row of response.reservations) {
                $('.calendar-table').append(`
                <tr id=cal-${row.id}>
                    <td>${row.id}</td>
                    <td>${row.beginsAt}</td>
                    <td>${row.endsAt}</td>
                    <td><button class="cancel-button" data-id=${row.id}>Zrušit</button></td>
                </tr>`);
            }

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function blockSpaceDialog(spaceNumber) {
    $('.blocking-dialog').remove();
    $(`#sp-${spaceNumber}`).after(
        `<tr class="blocking-dialog">
            <td>
                <form class="block-form" data-id=${spaceNumber}>
                    <label for="begins-at">Začátek:</label>
                    <input type="datetime-local" id="begins-at" name="begins-at" value="0">
                    <label for="ends-at">Konec:</label>
                    <input type="datetime-local" id="ends-at" name="ends-at" value="0">
                    <input type="submit">
                </form>
            <td>
        </tr>`
    )
}

function deleteSpace(id) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7036/api/Spaces/${id}`,
        dataType: "json",
        success: function () {
            $(`#sp-${id}`).remove();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function cancelReservation(id) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7036/api/Reservations/${id}`,
        dataType: "json",
        success: function () {
            $(`#res-${id}`).remove();
            $(`#cal-${id}`).remove();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function confirmReservation(id) {
    $.ajax({
        type: "PATCH",
        url: `https://localhost:7036/api/Reservations/${id}`,
        dataType: "json",
        success: function () {
            $(`#res-${id}`).remove();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function confirmAll() {
    $.ajax({
        type: "PATCH",
        url: `https://localhost:7036/api/Reservations/`,
        dataType: "json",
        success: function () {
            $("#requests").find("td").remove();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function unblock(id) {
    $.ajax({
        type: "DELETE",
        url: `https://localhost:7036/api/Blockings/${id}`,
        dataType: "json",
        success: function () {
            $(`#bl-${id}`).remove();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error:', textStatus, errorThrown);
        }
    });
}

function blockSpace(spaceNumber) {
    $("#make-reservation-result").empty();
    $.ajax({
        type: "POST",
        url: 'https://localhost:7036/api/Blockings',
        data: JSON.stringify({
            beginsAt: $('#begins-at').val() + "Z",
            endsAt: $('#ends-at').val() + "Z",
            spaceNumber: spaceNumber,
        }),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $('#block-space-result').append(`<p>Vytvořena blokace číslo ${data.id}.</p>`);
            displayBlocking(data);
        },
        error: function (data) {
            $('#block-space-result').append(`<p>${data.responseText}</p>`);
        }
    });
}

function displayBlocking(blocking) {
    $('#my-blockings').append(`<tr id=bl-${blocking.id}>
            <td>${blocking.id}</td>
            <td>${blocking.beginsAt}</td>
            <td>${blocking.endsAt}</td>
            <td>${blocking.spaceNumber}</td>
            <td>${blocking.createdAt}</td>
            <td>${blocking.adminId}</td>
            <td><button class="unblock-button" data-id=${blocking.id}>Zrušit</button></td>
        </tr>`);
}

function displaySpace(space) {
    $('#all-spaces').append(`<tr id=sp-${space.spaceNumber}>
            <td>${space.spaceNumber}</td>
            <td>${space.createdAt}</td>
            <td><button class="detail-button" data-space-number=${space.spaceNumber}>Detail</button></td>
            <td><button class="block-dialog-button" data-space-number=${space.spaceNumber}>Blokovat</button</td>
            <td><button class="delete-space-button" data-space-number=${space.spaceNumber}>Odstranit</button></td>
        </tr>`);
}
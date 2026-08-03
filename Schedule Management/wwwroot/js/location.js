$(document).ready(function () {

    // Country select hone par States lao
    $("#CountryId").change(function () {

        const countryId = $(this).val();

        $("#StateId").empty();
        $("#DistrictId").empty();

        $("#StateId").append(
            '<option value="">Select State</option>'
        );

        $("#DistrictId").append(
            '<option value="">Select District</option>'
        );

        if (!countryId) {
            return;
        }

        $.ajax({
            url: "/Location/GetStates",
            type: "GET",

            data: {
                countryId: countryId
            },

            success: function (states) {

                $.each(states, function (index, state) {

                    $("#StateId").append(
                        `<option value="${state.stateId}">
                            ${state.stateName}
                        </option>`
                    );
                });
            },

            error: function () {
                alert("Unable to load states.");
            }
        });
    });


    // State select hone par Districts lao
    $("#StateId").change(function () {

        const stateId = $(this).val();

        $("#DistrictId").empty();

        $("#DistrictId").append(
            '<option value="">Select District</option>'
        );

        if (!stateId) {
            return;
        }

        $.ajax({
            url: "/Location/GetDistricts",
            type: "GET",

            data: {
                stateId: stateId
            },

            success: function (districts) {

                $.each(
                    districts,
                    function (index, district) {

                        $("#DistrictId").append(
                            `<option value="${district.districtId}">
                                ${district.districtName}
                            </option>`
                        );
                    }
                );
            },

            error: function () {
                alert("Unable to load districts.");
            }
        });
    });

});
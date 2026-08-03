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

            $.each(districts, function (index, district) {
                $("#DistrictId").append(
                    `<option value="${district.districtId}">
                            ${district.districtName}
                        </option>`
                );
            });
        }
    });
});
});
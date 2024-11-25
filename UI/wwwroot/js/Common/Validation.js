function formValidation(ids = []) {
    var result = true;
    ids.forEach(id => {
        //let value = $("#" + id).val();
        if (!$("#" + id).valid()) {
            result = false;
            //    let span = DefaultService.createElement("span", { class: "span-smaller text-danger validation-text" }, [], { innerHTML: "<i class='fa-solid fa-circle-exclamation me-2'> </i>This Field is required" })
            //    $('#' + id).parent().append(span);
        }

    })
    return result
}
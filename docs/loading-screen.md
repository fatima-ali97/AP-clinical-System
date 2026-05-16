## how can u add the loading screen to ur views:

in the .cshtml ur working in add this:

##### partial name (its in `views/shared/_loadingOverlay`)

```html
<partial name="_LoadingOverlay" />
```

##### And the matching JS trigger (swap out the form/button IDs to match that view):

```js
javascript$("#yourFormId").on("submit", function () {
  if ($(this).valid()) {
    $("#blockui-overlay").css("display", "flex");
    $("#yourBtnId").prop("disabled", true).text("Please wait...");
  }
});
```

#### Quick tip — customizing the message per page: If you want different messages on different pages (e.g. "Loading records..." vs "Signing you in..."), you can pass a model into the partial like so:

```html
{{!-- Pass a custom message --}}
<partial name="_LoadingOverlay" model='"Loading patient records..."' />
```

Then in the partial, replace the hardcoded text with @Model. That way the animation and overlay are always reused, but the message is flexible per page.

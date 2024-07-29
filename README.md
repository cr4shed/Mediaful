## Mediaful
Mediaful is a Blazor Server social media application written in C# that was created with the goal of providing users with a meaningful way to track and share the movies and television shows they watch.

## Build Instructions
1. Clone the repository.
2. Set the connection string in `appsettings.json`.
3. Enter the required email details for `EmailSender.cs` in `Secrets.json`, `appsettings.json`, or hard-coded in the file itself. Alternatively, comment line 47 of `Program.cs` (the `EmailSender` service registration) to disable email functionality.
4. Run the project.

## Usage Instructions
1.	Navigate to the Register page.
2.	Enter credentials into the input fields and click the “Register” button.
3.	A verification email will be sent from `Mail:NoReplyAddress` to the entered email address, it is required to click the link in the email body to verify the account. This process may take several minutes and the email could be filtered to your “Junk” folder by mistake. If the `EmailSender` is disabled in `Program.cs`, instead click "Click here to confirm your account" to manually confirm your account.
4.	After verifying the account, navigate to the Login page and enter the credentials used to register.

## Screenshots
<details>
  <summary>Visitor View</summary>
  <br />
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/974e28e7-277c-4766-b088-b6626c569644">
</details>

<details>
  <summary>User View (inherit from Visitor)</summary>
  <br />
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/1daad12f-601f-487b-849c-5c927495f079">
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/c0f2023d-cfa7-4df2-948a-51af41dbc63a">
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/f6514bb9-f24a-4674-8e4d-a04cbf167596">
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/785e73d6-b11f-44e2-b1b3-7c6d3f66660d">
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/3f0468f0-906d-477f-8a95-6b37a1cf5fe3">
</details>

<details>
  <summary>Admin view (inherit from User)</summary>
  <br />
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/bfe6c192-6d21-4e96-ad7b-03c7e9de60fc">
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/b8779b98-8006-4385-aaad-9ca4c98fdc5e">
  <img src="https://github.com/cr4shed/Mediaful/assets/63892494/c84c99a5-f716-43ec-9df0-0e30ff361bb5">
</details>

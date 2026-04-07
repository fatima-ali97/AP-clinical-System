#### This .md file is meant to ONLY showcase the layout of the project with the functionality of each folder / file in it.

```
AP-clinical-system/
├── Controllers/
│   ├── HomeController.cs          ← Public website (landing, about, contact)
│   └── DashboardController.cs     ← App after login
│
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml           ← Landing page
│   │   ├── About.cshtml
│   │   └── Contact.cshtml
│   │
│   ├── Dashboard/
│   │   └── Index.cshtml           ← Main app page after login
│   │
│   └── Shared/
│       ├── _Layout.cshtml         ← Public site layout (navbar, footer)
│       ├── _AppLayout.cshtml      ← App layout (sidebar, app navbar)
│       └── _LoginPartial.cshtml   ← Login/logout buttons
│
└── wwwroot/
    ├── css/
    │   ├── site.css               ← Public site styles
    │   └── app.css                ← App styles
    └── js/

```

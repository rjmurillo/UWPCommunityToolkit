using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Toolkit.Services.MicrosoftGraph;

namespace Microsoft.Toolkit.Win32.UI.Controls.WinForms
{
    /// <summary>
    /// This class provides a simple Login functionality for the Microsoft Graph.
    /// It is implemented as a Windows Forms Component for a drag and drop experience from the toolbox.
    /// </summary>
    /// <seealso cref="IComponent" />
    public partial class GraphLoginComponent : Component
    {
        private string clientId;
        private string[] scopes;
        private GraphServiceClient graphServiceClient;
        private string displayName;
        private string jobTitle;
        private string email;
        private System.Drawing.Image photo;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphLoginComponent" /> class.
        /// </summary>
        public GraphLoginComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphLoginComponent" /> class.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> associated with the component.</param>
        public GraphLoginComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the ClientId of the application registered in the Azure AD V2 portal: http://apps.dev.microsoft.com
        /// </summary>
        public string ClientId { get => clientId; set => clientId = value; }

        /// <summary>
        /// Gets or sets the array of request Scopes for accessing the Microsoft Graph.
        /// Must not be null when calling LoginAsync
        /// </summary>
        public string[] Scopes { get => scopes; set => scopes = value; }

        /// <summary>
        /// Gets the default image for the logged on user from the Microsoft Graph
        /// </summary>
        public System.Drawing.Image Photo { get => photo; }

        /// <summary>
        /// Gets the display name for the logged on user from the Microsoft Graph
        /// </summary>
        public string DisplayName { get => displayName; }

        /// <summary>
        /// Gets the job title for the logged on user from the Microsoft Graph.
        /// </summary>
        public string JobTitle { get => jobTitle; }

        /// <summary>
        /// Gets the email address (UPN) for the logged on user from the Microsoft Graph.
        /// </summary>
        public string Email { get => email; }

        /// <summary>
        /// Gets the instance of the Microsoft.Graph.GraphServiceClient from the logon request
        /// </summary>
        public GraphServiceClient GraphServiceClient { get => graphServiceClient; }

        /// <summary>
        /// LoginAsync provides entrypoint into the MicrosoftGraphService LoginAsync
        /// </summary>
        /// <returns>A MicrosoftGraphService reference</returns>
        public async Task<bool> LoginAsync()
        {
            // check inputs
            if (string.IsNullOrEmpty(clientId))
            {
                // error
                return false;
            }

            // Initialize the MicrosoftGraphService
            if (!MicrosoftGraphService.Instance.Initialize(clientId, delegatedPermissionScopes: Scopes))
            {
                return false;
            }

            // Attempt to LoginAsync
            try
            {
                await MicrosoftGraphService.Instance.LoginAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            // Initialize fields from the User's information from the Microsoft Graph
            var user = await MicrosoftGraphService.Instance.GraphProvider.Me.Request().GetAsync();
            displayName = user.DisplayName;
            jobTitle = user.JobTitle;
            email = user.Mail;

            // get the profile picture
            using (Stream photoStream = await MicrosoftGraphService.Instance.GraphProvider.Me.Photo.Content.Request().GetAsync())
            {
                if (photoStream != null)
                {
                    photo = System.Drawing.Image.FromStream(photoStream);
                }
            }

            // return Microsoft.Graph.GraphServiceClient from the MicrosoftGraphService.Instance.GraphProvider
            graphServiceClient = MicrosoftGraphService.Instance.GraphProvider;
            return true;
        }
    }
}

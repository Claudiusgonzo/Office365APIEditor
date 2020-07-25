﻿// Copyright (c) Microsoft. All rights reserved. 
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information. 

using Microsoft.Identity.Client;
using System;
using System.Text;
using System.Windows.Forms;

namespace Office365APIEditor
{
    public partial class AcquireViewerTokenForm : Form
    {
        IPublicClientApplication _pca;
        AuthenticationResult _ar;

        public AcquireViewerTokenForm()
        {
            InitializeComponent();
        }

        private void AcquireViewerTokenForm_Load(object sender, EventArgs e)
        {
            Icon = Properties.Resources.DefaultIcon;

            button_UseBuiltInApp.Focus();
        }

        public DialogResult ShowDialog(out IPublicClientApplication pca, out AuthenticationResult ar)
        {
            DialogResult result = ShowDialog();

            pca = _pca;
            ar = _ar;

            return result;
        }

        private async void Button_UseMyApp_ClickAsync(object sender, EventArgs e)
        {
            if (!ValidateCustomClientID())
            {
                return;
            }

            await AcquireAccessTokenAsync(textBox_ClientID.Text);
        }

        private async void TextBox_ClientID_KeyDownAsync(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!ValidateCustomClientID())
                {
                    return;
                }

                await AcquireAccessTokenAsync(textBox_ClientID.Text);
            }
        }

        private bool ValidateCustomClientID()
        {
            if (textBox_ClientID.Text == "")
            {
                MessageBox.Show("Enter the Application ID.", "Office365APIEditor");
                return false;
            }

            return true;
        }

        private async void button_UseBuiltInApp_ClickAsync(object sender, EventArgs e)
        {
            await AcquireAccessTokenAsync(Properties.Settings.Default.BuiltInAppClientId);
        }

        private async System.Threading.Tasks.Task AcquireAccessTokenAsync(string ClientId)
        {
            Cursor = Cursors.WaitCursor;
            Enabled = false;

            string[] scopes = Util.MailboxViewerScopes();

            _pca = PublicClientApplicationBuilder.Create(ClientId).WithDefaultRedirectUri().Build();

            StringBuilder stringBuilder = new StringBuilder();

            try
            {
                stringBuilder.AppendLine("MSAL - AcquireTokenAsync");
                stringBuilder.AppendLine("Application ID : " + ClientId);
                stringBuilder.AppendLine("Scope : " + string.Join(",", scopes));

                _ar = await _pca.AcquireTokenInteractive(scopes).WithPrompt(Prompt.ForceLogin).ExecuteAsync();

                stringBuilder.AppendLine("Result : Success");
                stringBuilder.AppendLine("AccessToken : " + (_ar.AccessToken ?? ""));
                stringBuilder.AppendLine("ExpiresOn : " + _ar.ExpiresOn.ToString());
                stringBuilder.AppendLine("IdToken : " + (_ar.IdToken ?? ""));
                stringBuilder.AppendLine("Scope : " + string.Join(",", _ar.Scopes));
                stringBuilder.AppendLine("UniqueId : " + (_ar.UniqueId ?? ""));
                stringBuilder.AppendLine("Username : " + (_ar.Account.Username ?? ""));
                stringBuilder.AppendLine("Identifier : " + (_ar.Account.HomeAccountId.Identifier ?? ""));
                stringBuilder.AppendLine("Name : " + (_ar.Account.Username ?? ""));
                
                Properties.Settings.Default.Save();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine("Result : Fail");
                stringBuilder.AppendLine("Message : " + ex.Message);

                if (ex.Message != "User canceled authentication")
                {
                    MessageBox.Show(ex.Message, "Office365APIEditor");
                }
            }
            finally
            {
                Cursor = Cursors.Default;
                Enabled = true;
            }

            Util.WriteSystemLog("AcquireViewerTokenForm", stringBuilder.ToString());
        }

        private void LinkLabel_LearnMore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/microsoft/Office365APIEditor/tree/master/tutorials/Start_new_Mailbox_Viewer_Session.md");
        }
    }
}

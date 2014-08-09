using System;
using Novell.Directory.Ldap;


namespace Commerce_Courses_Student_List_Extractor
{
	/// <summary>
	/// Commerce Courses Student List Extractor is a simple application that allows an 
	/// user to search through objects on the Novell Network Tree of the University of 
	/// Cape Town (UCT) network via LDAP (Light-weight Directory Access Protocol) calls 
	/// and extract student lists from Course Group objects, using a specified course 
	/// group filter.
	/// 
	/// (Searches com.main.uct on UCT_T on rep1.uct.ac.za)
	/// </summary>
	class Student_List_Extractor
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length == 1)
			{
				Student_List_Extractor me = new Student_List_Extractor();
				Console.Write("Status Code: " + me.ldap_check(1, args[0]));
			}
			else
			{
				Console.WriteLine("Usage: Commerce Courses Student List Extractor.exe COURSECODE");
				Console.WriteLine("where COURSECODE is the AAANNNS (e.g. ECO321S) format course code you wish to extract student names from. The * character acts like a wildcard character.") ;
				Console.Write("Status Code: 0");
			}
		}

		private int ldap_check(int serveruse, string SearchParameter)
		{
			try
			{
				string ldapHost;
				int ldapPort;

				if (serveruse == 1)
				{
					ldapHost = "rep1.uct.ac.za";
					ldapPort = LdapConnection.DEFAULT_PORT;
				}
				else
				{
					ldapHost = "rep2.uct.ac.za";
					ldapPort = LdapConnection.DEFAULT_PORT;
				}
			
				// Creating an LdapConnection instance
				LdapConnection ldapConn= new LdapConnection();
				try				
				{
					//Connect function will create a socket
					ldapConn.Connect(ldapHost,ldapPort);
				}
				catch(System.Exception except)
				{
					Error_Handler(except);
					if (serveruse == 1)
					{
						Error_Handler("Server Connect Failure: Rep1 failed to reply.");
						ldap_check(2, SearchParameter);
						return 0;
					}
					else
					{
						Error_Handler("Server Connect Failure: Rep2 failed to reply.");
						return 0;
					}
				}
				string SearchContext = "com.main.uct";
				string stdn = "";
				string[] stempdn = SearchContext.Split('.');
				System.Collections.IEnumerator srunner = stempdn.GetEnumerator();
				while(srunner.MoveNext())
				{
					if (srunner.Current.Equals("uct") == false)
						stdn = stdn + "OU="+ srunner.Current + ",";
					else
						stdn = stdn + "O="+ srunner.Current + ",";
				}
				stdn = stdn.Remove(stdn.Length-1,1);
				
				if (stdn.Equals("") == true)
				{
					Error_Handler("Input Error: Search DN field invalid.");					
					return 0;
				}

				//Statusbar_Message("Search Parameter string set as " + SearchParameter.Text);
				if (SearchParameter.Equals("") == true)
				{
					Error_Handler("Input Error: Search Parameter field has been left blank.");
					return 0;
				}

				// Searches in the Marketing container and return all child entries just below this
				//container i.e Single level search
				LdapSearchQueue queue=ldapConn.Search(stdn,LdapConnection.SCOPE_SUB,
					"CN=" + SearchParameter + "_G",
					null,		
					false,
					(LdapSearchQueue) null,
					(LdapSearchConstraints) null );

				LdapMessage message;

				while ((message = queue.getResponse()) != null)
				{
					if (message is LdapSearchResult)
					{
						LdapEntry entry = ((LdapSearchResult) message).Entry;
						LdapAttributeSet attributeSet =  entry.getAttributeSet();
						
						System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();
						while(ienum.MoveNext())
						{
							LdapAttribute attribute=(LdapAttribute)ienum.Current;
							string attributeName = attribute.Name;
							if (attributeName.Equals("member") == true)
							{
								System.Collections.IEnumerator ienum2 = attribute.StringValues;
								while(ienum2.MoveNext())
								{
									char[] pars = {','};
									if (SearchParameter.IndexOf("*") > -1)
									{
										if (entry.DN.Split(pars)[0].Substring(3).Length == 9)
										Console.WriteLine(entry.DN.Split(pars)[0].Substring(3) + " " + ienum2.Current.ToString().Substring(3).Substring(0,9));
									}
									else
									{
										Console.WriteLine(entry.DN.Split(pars)[0].Substring(3) + " " + ienum2.Current.ToString().Substring(3).Substring(0,9));
									}
								}
							}
							
						}
					}
				}

				ldapConn.Disconnect();
			
				return 1;
			}
			catch(System.Exception except)
			{
				Error_Handler(except);
			}
			return 0;
		}

		private void File_Write(string message)
		{
			Console.WriteLine("Error Notice: " + message);
		}

		private void Error_Handler(System.Exception except)
		{
			File_Write(except.ToString());
		}

		private void Error_Handler(string except)
		{
			File_Write(except);
		}
	}
}

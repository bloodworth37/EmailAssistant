### Zach Bloodworth | zachbloodworth@gmail.com ###

# Email Assistant Project #
This is a personal project that I'm building to hone my full-stack development skills.
I've chosen the ASP.NET MVC framework to synergize with my experience using Azure 
cloud resources.

This application functions as an assistant which provides the user with valuable insights
into the contents of their email inbox. As of version 1.0, the primary feature is
"session review". This allows the user to create a session by selecting a start and end
date. The app will then proceed to query the Gmail API, retrieve all emails from within
that timeframe, parse the email JSON objects/extract relevant fields, and then upload
that information to the database. Once a session has been created, the user can access
a session overview. The session overview provides the user with a chronological list of
every email within the session, including metadata such as the subject, sender and date.
The session overview also includes a statistical summary of the emails - # of emails per
sender, # of emails per day, and avg/min/max daily emails. Additionally, when viewing the
email list, the user can access an AI-generated summary of each individual email.


## UPCOMING FEATURES ##
- UI/UX overhaul
    - CSS styling improvements
    - Layout improvements
    - Interactive JavaScript components
- AI summary page
    - Various high-level summary options
        - Summary by sender
        - Summary by day
        - Customizable summary
- Video demonstration
    - Link to a video demonstrating functionality
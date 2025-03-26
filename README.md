# Food Waste Reduction App

## About The Project
This project aims to reduce food waste by notifying users of discounts on near-expiration products at local supermarkets. It pulls data from the Salling Group API, categorizes offers by store and type, identifies top deals, and highlights significant offers. The application then compiles this data into an HTML email and sends it to subscribed users.

## Key Features

- **Data Fetching**: Retrieves live data on discounted food items using the Salling Group API.
- **Data Categorization**: Organizes food items by store and category.
- **Deal Identification**: Highlights the top five deals based on savings.
- **Email Notifications**: Sends out an HTML email with all the best offers and significant deals to users.
- **Console Summary**: Provides a detailed output in the console for verification and logging purposes.

## Built With

- C#
- .NET Core
- Newtonsoft.Json for JSON parsing
- SMTP for email notifications

## Getting Started

### Prerequisites

- .NET Core 3.1 or later
- A configured SMTP server or a Sendinblue API key for sending emails
- An API key from Salling Group for fetching food waste data

### Installation

1. **Clone the repository:**
   ```sh
   git clone https://github.com/your_username_/Project-Name.git

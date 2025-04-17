![MingleAllDevicesBanner 1 (2)](https://github.com/user-attachments/assets/ac66ba94-dad3-450a-b085-5011af846bab)


### 🎯 Project Purpose  

This project was initiated as part of the **BSM307 Computer Networks** course in the **Computer Technologies and Information Systems (CTIS)** department. The goal was to develop a real-time chat application as part of the course.  

Over time, the project was further developed by **Nazmi KOÇAK** and **Hamza Ali DOĞAN**, evolving into a more comprehensive application. After an extensive development process, the project was successfully completed on **March 25, 2025**.  

The primary purpose of this project was to help the development team successfully complete the **course** while gaining hands-on experience with a large-scale software implementation.  

Our main objective was to create a **user-friendly, minimalist, and modern real-time chat application** with a smooth and engaging user experience.  


### 🌐 Target Audience  

This project is designed for **software developers, students, employers, and web users** who are interested in real-time communication technologies.  

- **Software developers and students** can explore the project's architecture, WebSocket/WebRTC integration, performance optimizations, UI/UX design and modern website layout to improve their technical skills or get inspired.

- **Employers** can assess our software development skills, problem-solving abilities, and work discipline by reviewing the project's implementation.

- **Web users** can experience a **fast, user-friendly, and responsive** real-time chat application designed for seamless communication, including text, voice, and video chat.  

This project serves as an example of how **real-time messaging, group chats, multimedia sharing**, and **voice/video calling** can be efficiently implemented in a web-based environment.


![ProjectFeatures](https://github.com/user-attachments/assets/2be0bc9a-aa47-49d6-bb78-bd8234286059)

![mingle runasp net_swagger_index html](https://github.com/user-attachments/assets/25a112c3-3d48-4b15-a0bf-c98242b34a1d)


### 🔐 **Authentication & Authorization**
- Account creation with email and password.
- Quick registration using **Google** and **Facebook**.
- Password reset via email.
- JWT based secure login.
___

### 💬 **Real-Time Messaging**
- Real-time **group** and **individual** messaging support.
- Message **status** tracking (sent, delivered, read).
- AI-integrated response generation.
- File and media attachment support.
- Options to **delete** and **copy** messages.
___

### 📞 **Voice and Video Calls** 📱🎥
- One-to-one voice and video calls with registered users.
- Mute microphone and toggle speaker on/off.
- Call logs tracked with status and type.
___

### ✨ **AI-Generated Content**
- Integrated with external AI models (Gemini, Flux, Artples, Compvis).
- **Text & image** generation via API endpoints.
- **Rebuilding**, **liking** and **copying** AI responses.
___

### 🧾 **Call Logs and Archives**  
- Full call history tracking that can be cleared on demand.
- **Archiving** and **unarchiving** options for individual chats.
___

### 👥 **Group Management**  
- Group creation and **role-based** permissions.
- Group admin controls (add, remove, update roles).
- Ability to **leave** the group at any time.
___

### ☁️ **Media & File Uploads**  
- **File processing** for optimized delivery.
- MIME/type validation, secure upload, and retrieval.
- Supported: **DOC, XLSX, TXT, ZIP, MP3, WEBP, PNG etc**.
___

### 🔧 **Configuration and Error Handling**  
- Field/File validation helpers.
- **Clean** and **scalable** structure.
- Global exception handling through dedicated exception middleware.
___

### ⚙️ **Settings**
- User account management.
- Theme customization. 
- Security settings.
- Help & support
___

### 🔑 **Message Encryption**
- **End-to-end encryption** for secure communication.
- All chat messages are encrypted for enhanced privacy.
___

### 🔍 **Search**
- Search for specific users, groups, calls, and archives.
___

### 📱 **Device Compatibility**  
- Works well on both **desktop** and **mobile devices**.  
- Adjusts to different screen sizes for a smooth experience.  
___

![369714386-693d2a0b-b493-4e18-befa-da1365294af0](https://github.com/user-attachments/assets/03ca933f-35ce-420b-9412-9e56917543e0)

### 🖥️ **ASP.NET Core Web API**
ASP.NET is a robust framework for building backend APIs. It serves as the backbone of this project, handling data processing, user authentication, and real-time communication with the frontend.

### ⚛️ **React**
React is a JavaScript library for building user interfaces. It helps create dynamic and reusable components, enhancing the user experience and making the application more maintainable.

### 🎨 **Sass/SCSS**
Sass/SCSS is a powerful and organized version of CSS. It provides advanced features like variables, nesting, and mixins, helping to structure and maintain large-scale CSS more effectively.

### 🔑 **Firebase Authentication**
Firebase Authentication is used to handle user authentication securely. It allows users to sign up and log in using email/password or third-party services like Google and Facebook.

### 🔥 **Firebase Realtime Database**
Firebase Realtime Database enables real-time synchronization of data across all clients. It is used to store and manage user data, chat history, call logs, and group records efficiently.

### ☁️ **Cloudinary**
Cloudinary is a powerful cloud-based media management service used for storing, optimizing, and delivering images and files. In this project, it handles media uploads, transformations, and fast content delivery, ensuring efficient and scalable asset management.

### 🔌 **WebSocket**
WebSocket provides full-duplex communication channels over a single TCP connection, enabling real-time, bidirectional communication between the frontend and backend, crucial for live messaging and updates.

### 🎥 **WebRTC**
WebRTC enables real-time peer-to-peer communication, including voice and video calls. It allows users to connect with each other seamlessly for interactive communication.

### ⚡ **Vite**
Vite is a fast and efficient build tool for modern web projects. It enables quick reload and builds, improving the development workflow and integrating smoothly with React.

### 🌐 **Netlify**
Netlify is a platform for deploying and hosting the project. It automates the deployment process, ensuring the app is live and served securely on the web.

### 🗄️ **Monster ASP.NET**
It is a platform focused on hosting ASP.NET and .NET Core applications. It automates the deployment process, ensuring the app is live and served securely on the web.

![projectdependencies](https://github.com/user-attachments/assets/6448ea9b-da46-40e1-b4a5-b039ac4ec629)

### 📁 **Mingle.API**
- Microsoft.AspNetCore.SignalR
- Swashbuckle.AspNetCore (Swagger)
- Microsoft.AspNetCore.Authentication.JwtBearer

### 📁 **Mingle.Core**
- Microsoft.Extensions.Configuration
- Microsoft.IdentityModel.JsonWebTokens
- Microsoft.IdentityModel.Tokens

### 📁 **Mingle.DataAccess**
- CloudinaryDotNet
- FirebaseDatabase.net
- FirebaseAuthentication.net
- Microsoft.AspNetCore.Http.Features
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Options

### 📁 **Mingle.Services**
- AutoMapper

### 📁 **Mingle.Entities & Shared**
- There are no NuGet packages installed.

![ProjectProcess](https://github.com/user-attachments/assets/ddf11844-0b92-40fb-b065-0062a101ceac)

###  **1. Idea**  
The first step was to think about the **main purpose** of the project and what makes it unique.  
We focused on:  
- Understanding **what problem it solves** for users.  
- Finding ways to make it **different** from other chat applications.  
- Making sure the idea is **realistic, useful, and can grow** in the future.  

This phase helped us create a clear vision for the project.  
___

### **2. Research Process**  
In this phase, we analyzed **existing real-time chat applications** to learn from them.  
We focused on:  
- Examining how other chat applications **work**.  
- Identifying **their weaknesses** and missing features.  
- Finding useful features that we could **adapt and improve**.  

This research helped us design a more **efficient and competitive** chat application.  
___

###  **3. Project Features**  
In this phase, we focused on defining the **core features** of the application and aimed to create a **unique and superior user experience** compared to existing chat applications. After analyzing the current market and competitors, we identified several key features that would set our project apart.  

The application will offer **real-time messaging** with **sent, delivered, and read** message statuses, allowing users to always know the status of their messages. Both **individual and group chats** will be supported, with additional features for **group member management**, **message history**, and **real-time updates**. **Voice and video calls** will be integrated, with features like muting and toggling the speaker to improve the communication experience.  

Additionally, users will have access to **AI-powered chat**, allowing them to generate text and images directly from the chat interface, offering a modern and interactive experience. **File sharing** will also be a priority, enabling a wide range of file types to be sent and received in both individual and group chats. To help users better organize their conversations, we will include options for **archiving** and **unarchiving** chats, as well as maintaining **call logs**, which will display details such as call time, duration, and type.  

By combining all of these features, we aim to create an **innovative and user-centered** messaging application that not only meets users' basic needs but also offers advanced functionalities that competitors often lack.  
___

### **4. Database & API Design**  
The core backend functionalities were structured under the following main features:

- Real-time messaging
- Voice and video calling with signaling management
- User management (registration, login, authentication, roles, token management)
- Group chat management (adding/removing members, group information)
- Message statuses (sent, delivered, read)
- File upload/download APIs
- AI-powered content generation
- Call history and logging systems
- Archived/deleted chat management

All API endpoints were designed following RESTful principles and made documentable via OpenAPI/Swagger.
   
- The backend architecture is structured around robust database and API design principles to support seamless real-time communication and data management.
- A **NoSQL** database was selected to ensure scalability, flexibility, and high performance for storing chat data, user information, and media assets.
- Key database entities and their relationships were carefully designed to align with the application's modular structure.
- Both an **Entity-Relationship (ER)** diagram and a detailed relational schema were created to illustrate data flows and interactions.
- The schema includes collections for users, messages, conversations, groups, calls, and logs, each optimized for rapid access and minimal latency.

This comprehensive database design ensures that the real-time nature of the application is supported by a high-performance data layer.

![Frame 223 (3)](https://github.com/user-attachments/assets/ba0f52eb-8409-4942-99b4-880a9745e304)
___

### **5. Choosing Technologies to Meet Project Requirements**
In this phase, we focused on selecting the best technologies to help the project reach its goals. Our main aim was to make sure the app is scalable and efficient, and can handle real-time communication, security, and smooth performance.

We looked at different technologies to see if they could meet needs like **real-time messaging**, **user authentication**, and **performance optimization**. After reviewing the options, we chose the best technologies that would allow us to develop a secure, scalable, and smooth-working app.

This phase was an important step for the whole project. By choosing the right tools and frameworks, we made sure the app could be developed effectively and meet both user and technical needs.
___

### **6. Starting the Project**
In this phase, after finalizing all the ideas, resources, and technologies to be used, as well as completing the initial planned designs, we began the project. This stage involved setting up the project environment, installing necessary dependencies, and configuring the project structure to ensure everything was ready for development.

By making sure all configurations and setups were in place, we ensured a smooth start to the coding process and the overall development of the project.
___

### **7. Project Folder Structure**
In this phase, we focused on creating a clear and organized folder structure for the project. The goal was to make it easy to manage and navigate through the project’s files. We carefully decided how to organize the codebase, making sure that the folders were logically named and categorized.

By setting up the folder structure in an efficient way, we ensured that the project would be scalable and maintainable in the long run. This organization helped both the development team and future contributors easily find and update the necessary files without confusion.

![klasör](https://github.com/user-attachments/assets/613156b6-a554-4aad-96a1-1ef7e02d86f2)

___

### **8. Breaking the Project into Phases** 
To manage the development process effectively, we divided the project into different phases. Each phase focused on a specific feature or improvement, ensuring a structured and step-by-step approach.  

We first prioritized the **core features**, such as real-time messaging and authentication. Then, we planned additional functionalities like group chats, file sharing, and voice/video calls. This helped us stay organized and work efficiently without overwhelming the development process.  

We also identified **reusable components** that could be used across the project, making development faster and maintaining consistency in the UI. By breaking the project into clear phases, we ensured a smooth and well-structured workflow.  
___

### **9. Coding Process**  
This stage marked the transition from architectural planning to the implementation of core backend functionalities. Development proceeded in alignment with the predefined workflow and prioritized feature list, ensuring a systematic and modular codebase structure.

A critical aspect of this phase involved translating conceptual designs and technical specifications into actual service implementations, primarily focusing on the integration of real-time communication protocols, user identity infrastructure, and secure data handling workflows. Leveraging the **ASP.NET Core Web API** framework and **.NET 8** runtime, we constructed controllers, services, and data access layers that adhered to **SOLID** principles and clean architecture conventions.

Throughout development, the team conducted extensive research to identify the most efficient solutions for each requirement. This included evaluating existing NuGet packages and third-party libraries for functionalities such as **JWT** authentication, **SignalR** for real-time communication, and **WebRTC** for multimedia streaming.

Functionality was implemented incrementally and accompanied by unit and integration testing to ensure system stability and correctness. Special attention was given to API versioning, error handling strategies, and consistent **RESTful** endpoint design, all of which were documented via **OpenAPI (Swagger)** specifications.

___

### **10. Evaluating Progress**  
As the coding process continued, we regularly stopped to **evaluate the completed features** and discuss possible improvements. This phase was important for ensuring that everything was working as expected and aligned with our initial goals.  

We tested different parts of the application, reviewed the **user experience**, and checked for any **performance issues**. If we found areas that could be improved, we discussed possible solutions and adjusted our approach.  

During this phase, we also revisited the **new feature ideas** that came up while coding. We carefully analyzed whether these features would add real value to the project and if they were feasible to implement. Some of them were added immediately, while others were planned for future updates.  

By continuously evaluating our progress, we ensured that the project stayed on the right track and delivered the best possible user experience.

___

### **11. Completing the Coding Process**  
At this stage, we finalized the development of all planned backend features and ensured that the system was fully functional and production-ready. After implementing the core services—including real-time messaging, authentication, media signaling, and user/group management—we conducted a thorough codebase review to identify redundant logic, enhance maintainability, and improve overall code quality.

We then transitioned into the performance optimization phase, focusing on techniques to improve backend efficiency, scalability, and response times. Key areas of optimization included:

- Refactoring complex queries and data access logic to reduce latency.
- Implementing caching mechanisms (e.g., in-memory caching, response caching) to decrease database load.
- Optimizing SignalR performance for stable real-time messaging at scale.
- Minimizing I/O operations and improving asynchronous execution for better throughput.

In parallel, we used logging and profiling tools to analyze potential performance bottlenecks across API endpoints and middleware pipelines. We also stress-tested various services to ensure consistent behavior under high concurrency and load.

___

### **12. Performance and Optimization**  

During this phase, our primary focus was on making the backend system more **scalable**, **reliable**, and **efficient** by applying a variety of performance optimization strategies. The goal was to ensure the application could handle increased load, deliver faster response times, and maintain stability under different operating conditions. Below are the key techniques we implemented:

- **Database Indexing and Query Optimization:** We analyzed frequently used queries and added indexes to key fields to reduce query time. We also refactored slow or repetitive queries, especially for real-time messaging and user search features.

- **Connection Management:** For SignalR and WebRTC signaling, we optimized connection lifetimes and resource management to ensure low-latency communication and stable session handling, even with concurrent users.

- **Asynchronous Task Handling:** We offloaded time-consuming processes like file storage, message archiving, and logging to background services using asynchronous patterns and queuing systems. This reduced blocking and improved throughput for the main API threads.

- **Request Throttling and Rate Limiting:** We added rate-limiting middleware to protect the API from abuse and ensure fair usage. This was especially important for authentication endpoints and messaging routes.

These backend-focused enhancements greatly improved **system responsiveness**, **resource utilization**, and **scalability**, allowing the application to support a larger user base while maintaining a seamless and real-time experience.

___

### **13. Security and Testing Process**  

At this stage, we focused on ensuring the **security, stability, and reliability** of our application before making it available to real users.  

#### **Security Measures**  
One of the most critical aspects of this phase was securing user data and preventing unauthorized access. Since we used **Firebase**, we implemented strict **Firestore security rules** to control data access and ensure that users could only interact with the data they were authorized to. We carefully defined these rules to protect personal information, messages, and other sensitive data.  

#### **API Testing with Postman**
To validate the backend functionality, we used **Postman** to test both **HTTP** and **WebSocket** endpoints. This allowed us to simulate real-world interactions such as user login, message sending, file uploads, and WebRTC signaling. By testing with Postman, we ensured our APIs responded correctly, handled errors gracefully, and maintained proper authentication and authorization protocols. WebSocket-based messaging scenarios were also verified to confirm real-time message delivery and status updates.

#### **Testing with Real Users**  
To understand how the application performs in real-world scenarios, we conducted tests with real users. These tests helped us gather valuable feedback and identify any unexpected behaviors that might affect the user experience.  

#### **Testing Different Scenarios**  
We performed various test cases to ensure the system worked seamlessly across different conditions, including:  
- **Multi-device testing** – A single user logging in from multiple devices (e.g., phone, tablet, and desktop) to verify real-time synchronization.  
- **Cross-browser testing** – Running the app on different browsers (Chrome, Firefox, Edge, Safari) to check for compatibility issues.  
- **Network conditions testing** – Evaluating how the app handles slow or unstable internet connections.  

Throughout this phase, we **monitored system behavior**, analyzed logs, and fixed any detected issues to ensure our **authentication, real-time messaging, and security systems** worked flawlessly.  

With security and stability confirmed, we were ready for the next step:  
**Deployment Phase**, where we prepared to launch our application for public use.

___

### **14. Deployment Phase**  

The deployment phase marked the final stage before releasing the backend services to production. This phase was crucial in ensuring that our API infrastructure was stable, secure, and ready to support real-world usage at scale.

#### Deploying on Monster ASP.NET  
We deployed the backend on **Monster ASP.NET**, a reliable Windows-based hosting provider that supports ASP.NET Core applications. To automate the deployment process, we integrated **GitHub Actions** into our **CI/CD** pipeline. With every new push to the main branch, GitHub Actions would **automatically build the project** and **deploy it to the Monster ASP.NET server**, ensuring that the latest code was always live and consistent across environments.

#### SSL Certificate Integration
To ensure secure communication across the application, we configured and applied a valid SSL certificate on the server. This secured all API endpoints with **HTTPS**, which is especially critical for **WebRTC signaling**, **authentication**, and **file transfer** operations.

#### Challenges & Solutions
While setting up automated deployment, we encountered challenges related to **build configuration compatibility** and **server file access permissions**. These were addressed by customizing the build scripts within GitHub Actions and adjusting the hosting environment’s security policies to allow smooth deployments and runtime operations.

With this setup, the backend became **robust**, **automated**, and **production-ready**, enabling real-time features and secure communication to function seamlessly alongside the frontend.

#### CORS Configuration  
We also updated and configured **CORS (Cross-Origin Resource Sharing)** settings across the project to allow safe interaction between the frontend and backend services. This was essential to ensure that data could flow correctly between the client and server, even when hosted on different domains or environments.

By resolving these issues, we successfully deployed the project, making sure everything worked correctly, from data handling to real-time communication. The **application** is now live and functioning smoothly, ready for users to start interacting with it.

With the deployment complete, we moved on to the final phase:
**User Feedback and Maintenance Phase**, where we monitor user interactions and improve the application further.  
___

### **15. User Feedback and Maintenance Phase**  
This phase focuses on gathering real user feedback, suggestions, and addressing any bugs or issues that arise after the deployment of the application. It's an ongoing process where we continuously monitor the application's performance in real-world conditions.  

After launching the app, we encouraged users to provide feedback on their experience, which helped us identify areas for improvement. Based on this feedback, we worked on fixing bugs, improving user interface elements, and optimizing the overall experience.  

We also monitored the application to catch any unforeseen issues or bugs that might not have been detected during the testing phase. Once identified, we swiftly addressed these issues to ensure that the application remained stable and user-friendly.  

This phase is crucial because it ensures the application keeps evolving to meet user expectations and remains reliable in the long term. By implementing user feedback and regularly improving the app, we maintained high levels of user satisfaction and ensured continuous progress in the development process.

![VersionAndDeveloper](https://github.com/user-attachments/assets/3f21a437-7059-467b-8f39-9d9c5715965a)

- **Release Date**: 25.03.2025  

- **Last Updated**: 29.03.2025 🕟 12:00  

- **Version**: 1.000.0.0  

- **License**: MIT License  

- **Supported Browsers**:  
  The project is compatible with popular web browsers like Chrome, Firefox, Safari, and Edge in their latest versions.  

- **Project Link**:  
  Access the project by clicking [here](https://mingleweb.netlify.app/).  

- **Developers**:  
  The **backend** of the project was developed by **Nazmi KOÇAK**. For more information about the developer, please visit the [LinkedIn profile](https://www.linkedin.com/in/nazmikocak/).  
  The **frontend** of the project was developed by **Hamza Ali DOĞAN**. For more details, you can visit the [backend repository](https://github.com/HamzaDogann/Mingle).  

- **Feedback and Support**:  
  If you have any feedback or need support, feel free to contact me at [nazmikocak.dev@hotmail.com].  
  For questions or issues related to the frontend, you can reach out to **Hamza Ali DOĞAN** at [hamzaalidogantr@gmail.com].  

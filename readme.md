
# WebCommander

This or `WebCommander.exe` is a client side application, that will execute pre stored queries as triggered by the master (coming soon).

Request will triggered from the websocket server. Which is in development, https://github.com/ABugNamedBeetle/SimpleWebSocketJS


## Tech Stack

**Client:** C# (compiled to native using bflat)

**Server:** Node, Express (hosted on glitch.me)

**IDE:** Im only using VS Code with C# extension (VS can also be used, in AOT also ).


## Demo
https://github.com/ABugNamedBeetle/WebCommander/assets/62416701/22d668a3-8a9e-4076-88dc-f2c6e4621cb3


## Build
- To build using `bflat` in VS code, `bb.ps1` is given, add the following in in tasks.json,  and execute the task. 
    
    ```json
       {
            "label": "Build Bfalt",
            "type": "shell",
            "command": "powershell",
            "args": [
                "./bb.ps1",
                "-Folder",         "App",
                "-OutputName", "WebCommander.exe"
                
            ],
             "problemMatcher": "$msCompile",
        }
    ```
    or, can be build using
    ```shell
    ./bb.ps1 -Folder "App" -OutputName "WebCommander.exe"
    ```
    
- To build using VS, you know the drill 😎

## Roadmap

- only basic text are being sent sent from server to client 🤣.

- JSON support in both `client` and `server` is still in development, like a 🔥.

- security for now is not my major concern for now 🤔.

## Contributing

Contributions are always welcome!

See `contributing.md` for ways to get started.

Please adhere to this project's `code of conduct`.


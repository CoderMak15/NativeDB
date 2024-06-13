const express = require('express');
const { Pool, Client } = require('pg');
const http = require('http');
const net = require('net');
const dgram = require('dgram');

const app = express();
const port = 3000;

// PostgreSQL connection configuration
const pool = new Pool({
  user: 'postgres',
  host: '10.0.0.121',
  database: 'postgres',
  password: 'password',
  port: 5432,
});

class User {
    constructor(Username, Password) {
      this.Username = Username;
      this.Password = Password;
    }
  }

User.fromJSON = function (json){
    let user = JSON.parse(json)
    return new User(user.Username, user.Password)
}

async function connectDb(user, socket){
    try {
        const client = new Client({
            user : "postgres",
            database: "postgres",
            port: 5432,
            host: "localhost",
            password: "password"
        })
        // console.log(client)
        await client.connect();
    
        console.log("Client Connected")
        // Parameterized query to check the username and password
        const result = await client.query(
          'SELECT * FROM public."Users" WHERE username = $1 AND password = $2',
          [user.Username, user.Password]
        );
    
        await client.end();
    
        if (result.rows.length > 0) {
          // User exists and password matches
          console.log(result.rows[0])
          socket.write(JSON.stringify({ message: 'Login successful', user:result.rows[0]  }))
        } else {
          // User not found or password doesn't match
          socket.write({ message: 'Invalid username or password' })
        }
      } catch (err) {
        console.error(err);
        socket.write('Error 500 fetching data from the database');
      }
}

const tcpServer = net.createServer((socket) => {
    console.log('TCP client connected:', socket.remoteAddress + ':' + socket.remotePort);
  
    // Respond when client sends data
    socket.on('data', (data) => {
        var user = User.fromJSON(data.toString())
        console.log(user)
        connectDb(user, socket)


    //   socket.write('Hello TCP client, you connected from ' + socket.remoteAddress + ':' + socket.remotePort + '\r\n');
    });
  
    // Handle client disconnect
    socket.on('end', () => {
      console.log('TCP client disconnected:', socket.remoteAddress + ':' + socket.remotePort);
    });
  
    // Handle errors
    socket.on('error', (err) => {
      console.error('TCP socket error:', err.message);
    });
  });
  
  tcpServer.listen(3000, () => {
    console.log('TCP server is listening on port', 3000);
  });


app.get('/', (req, res) => {
  res.send('Welcome to the Node.js server!');
});

app.post('/login', async (req, res) => {
    console.log("req : " + req.body)
    console.log("res : " + res)

    const { username, password } = req.body; // Extract username and password from request body
  
    // Basic input validation
    if (!username || !password) {
      return res.status(400).send('Username and password are required');
    }
  
    try {
      const client = await pool.connect();
  
      // Parameterized query to check the username and password
      const result = await client.query(
        'SELECT * FROM users WHERE username = $1 AND password = $2',
        [username, password]
      );
  
      client.release();
  
      if (result.rows.length > 0) {
        // User exists and password matches
        res.status(200).json({ message: 'Login successful', user: result.rows[0] });
      } else {
        // User not found or password doesn't match
        res.status(401).json({ message: 'Invalid username or password' });
      }
    } catch (err) {
      console.error(err);
      res.status(500).send('Error fetching data from the database');
    }
  });

// http.createServer(app).listen(port, '0.0.0.0', () => {
//   console.log(`Server is running on http://0.0.0.0:${port}`);
// });

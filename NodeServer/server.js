const { Pool } = require('pg');
var md5 = require('js-md5');
const net = require('net');


const port = 3000;

const client = new Pool({
  user : "postgres",
  database: "postgres",
  port: 5432,
  host: "localhost",
  password: "password"
})

class User {
    constructor(Username, Password) {
      this.Username = Username;
      this.Password = Password;
    }
  }

User.fromJSON = function (login){
  console.log(login)
    return new User(login.username, login.password)
}

function login(data, socket){
  let user = User.fromJSON(data)
  let encryptedPw = md5(user.Password)
  let query = "SELECT * FROM public.\"Users\" WHERE username = \'" + user.Username + "\' AND password = \'" + encryptedPw + "\'"
  console.log(query)
  querydb(query, socket)
}

async function querydb(query, socket){
    try {
        const result = await client.query(query)
    
        if (result.rows.length > 0) {
          socket.write(JSON.stringify({ status: 200, payload: result.rows[0]}))
        } else {
          socket.write(JSON.stringify({ status: 403, payload: 'Invalid Query or param' }))
        }
      } catch (err) {
        socket.write(JSON.stringify({ status: 500, payload: 'Error while fetching data from the db' }))
      }
}

const tcpServer = net.createServer((socket) => {
    console.log('TCP client connected:', socket.remoteAddress + ':' + socket.remotePort);

    socket.on('data', (data) => {
      console.log(data)
      let payload = JSON.parse(data)
      console.log(payload)
      if (payload.message === "Login"){
        login(payload.login, socket)
      }
      else {
        socket.write({ status: 403, payload: 'End Point undefined'})
      }
    });
  
    socket.on('end', () => {
      console.log('TCP client disconnected:', socket.remoteAddress + ':' + socket.remotePort);
    });
  
    socket.on('error', (err) => {
      console.error('TCP socket error:', err.message);
    });
  });
   
  tcpServer.listen(port, () => {
    client.connect();
    console.log('TCP server is listening on port', port);
  });

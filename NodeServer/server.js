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
    return new User(login.username, login.password)
}

async function login(data, socket){
  let user = User.fromJSON(data)
  let encryptedPw = md5(user.Password)
  let query = "SELECT * FROM public.\"Users\" WHERE username = \'" + user.Username + "\' AND password = \'" + encryptedPw + "\'"
  let result = await querydb(query)
  console.log(result)
  socket.write(JSON.stringify(result))
}

async function create(data, socket){
  let user = User.fromJSON(data)
  let query = "SELECT * FROM public.\"Users\" WHERE username = \'" + user.Username + "\'"
  let result = await querydb(query)

  if (result.status == 200){
    socket.write(JSON.stringify({ status: 409, payload: 'User already exists' }))
    return
  }

  let encryptedPw = md5(user.Password)
  query = "INSERT INTO public.\"Users\"(username, password, access, name, surname, email, password_modification) VALUES (\'" + user.Username+ "\', \'" + encryptedPw + "\', False, '', '', '', NOW());"
  result = await querydb(query)
  socket.write(JSON.stringify(result))
}

async function movies(socket){
  let query = "SELECT name, description, url FROM public.\"Movie\""
  let result = await querydb(query)
  console.log(result)
  socket.write(JSON.stringify(result))
}

async function querydb(query){
    try {
        const result = await client.query(query)
        let response = null

        if (result.rowCount > 0) {
          if(result.rows.length > 0){
            response = { status: 200, payload: result.rows}
          } else {
            response = {status: 200, payload: true}
          }
        } else {
          response = { status: 403, payload: 'Invalid Query or param' }
        }
        return response

      } catch (err) {
        return { status: 500, payload: 'Error while fetching data from the db' }
      }
}

const tcpServer = net.createServer((socket) => {
    console.log('TCP client connected:', socket.remoteAddress + ':' + socket.remotePort);
    socket.on('data', (data) => {
      let payload = JSON.parse(data)

      switch(payload.message){
        case "Login":
          login(payload.login, socket)
          break
        case "Create":
          create(payload.login, socket)
          break
        case "Movies":
          movies(socket)
          break
        default:
          socket.write({ status: 403, payload: 'End Point undefined'})
          break
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

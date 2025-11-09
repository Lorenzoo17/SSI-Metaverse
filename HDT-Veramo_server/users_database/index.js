const express = require('express')
const sqlite3 = require('sqlite3').verbose()

const app = express()
const port = process.env.PORT || 3000

const db = new sqlite3.Database('./users.db', (err)=>{
    if(err){
        console.error(err.message)
    }

    // console.log("Correctly connected to database")
});

app.use(express.json())

app.get('/users', (req, res) => { // curl -X GET http://localhost:3000/users
    db.all('SELECT * FROM user', (err, rows) => {
        if (err) {
            console.error(err.message);
            res.status(500).send('Internal server error');
          } else {
            res.send(rows);
          }
    });
});

app.get('/users/:alias/:psw', (req, res) => { // curl -X GET http://localhost:3000/users/Gianni/default/
  const alias_specified = req.params.alias
  const psw_specified = req.params.psw

  const query = 'SELECT * FROM user WHERE alias = ? AND psw = ?'
  db.all(query, [alias_specified, psw_specified], (err, rows) => {
    if(err){
      res.status(500).send(err.message)
    }
    else{
      if(rows.length == 1){
        res.status(200).send('1')
      }else{
        res.status(500).send('Login not working')
      }
    }
  });
});

app.post('/users', (req, res) => { // curl -X POST http://localhost:3000/users -H "Content-Type: application/json" -d "{\"alias\":\"Lorenzo\",\"psw\":\"default\"}"
    const { alias, psw } = req.body;
    if (!alias || !psw) {
      res.status(400).send('Alias and password are required');
    } else {
      const sql = 'INSERT INTO user(alias, psw) VALUES (?, ?)';
      db.run(sql, [alias, psw], function(err) {
        if (err) {
          console.error(err.message);
          res.status(500).send('Internal server error');
        } else {
          const id = this.lastID;
          res.status(201).send({ id, alias, psw });
        }
      });
    }
});

app.delete('/users/:id', (req, res) => { // curl -X DELETE http://localhost:3000/users/1 ==> 1 here is the id!
    const { id } = req.params;
    db.run('DELETE FROM user WHERE id = ?', [id], function(err) {
      if (err) {
        console.error(err.message);
        res.status(500).send('Internal server error');
      } else if (this.changes === 0) {
        res.status(404).send('Product not found');
      } else {
        res.status(204).send();
      }
    });
  });
  

app.listen(port, ()=>{
    console.log("HTTP server stared listening from port : " + port)
});
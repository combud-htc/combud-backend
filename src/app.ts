import Express from "express";
import Http from "http";
import Winston from "winston";
import Bp from "body-parser";
import Knex from "knex";
import User from "./models/User";

const app = Express();

// Middleware
app.use(Bp.urlencoded({"extended" : false}));
app.use(Bp.json({"strict" : true}));

const server = Http.createServer(app);

// Setup logger
const logger = Winston.createLogger({
	"transports" : [
		new Winston.transports.Console({
			"format" : Winston.format.cli()
		}),
		new Winston.transports.File({
			"format" : Winston.format.simple(),
			"filename" : "logs/logs.log"
		})
	]
});

const db = Knex({
	"client" : "mysql",
	"connection" : {
		"host" : process.env.DB_HOST,
		"user" : process.env.DB_USER,
		"password" : process.env.DB_PASSWORD,
		"database" : process.env.DB_NAME
	}
});

db.on("error", logger.error);

server.listen(parseInt(process.env.PORT || "3000", 10), () => logger.info(`Listening on port ${process.env.PORT || "3000"}!`));
import Express from "express";
import Http from "http";
import Winston from "winston";
import Bp from "body-parser";
import Knex from "knex";

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

server.listen(parseInt(process.env.PORT || "3000", 10), () => logger.info(`Listening on port ${process.env.PORT || "3000"}!`));
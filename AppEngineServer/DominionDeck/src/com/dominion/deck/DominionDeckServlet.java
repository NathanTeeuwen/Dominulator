package com.dominion.deck;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Date;
import java.util.Enumeration;
import java.util.Iterator;
import java.util.Random;
import java.util.TreeSet;

import javax.servlet.http.*;

import com.google.appengine.api.datastore.DatastoreService;
import com.google.appengine.api.datastore.DatastoreServiceFactory;
import com.google.appengine.api.datastore.Entity;
import com.google.appengine.api.datastore.FetchOptions;
import com.google.appengine.api.datastore.KeyFactory;
import com.google.appengine.api.datastore.PreparedQuery;
import com.google.appengine.api.datastore.Query;
import com.google.appengine.api.datastore.Query.CompositeFilterOperator;
import com.google.appengine.api.datastore.Query.Filter;
import com.google.appengine.api.datastore.Query.FilterOperator;
import com.google.appengine.api.datastore.Query.FilterPredicate;
import com.google.appengine.api.datastore.Query.SortDirection;
import com.google.gson.Gson;

// Local Request:
// http://localhost:8888/dominiondeck?action=RECORD&values={%22deck%22:{%22useShelters%22:false,%22useColonyAndPlatinum%22:false,%22kingdomPiles%22:[%22Haven%22,%22Hermit%22,%22Pirate%20Ship%22,%22Feodum%22,%22Death%20Cart%22,%22Junk%20Dealer%22,%22Ill-Gotten%20Gains%22,%22Merchant%20Guild%22,%22Stash%22,%22Nobles%22],%22events%22:[]},%22expansions%22:[{%22name%22:%22Dark_Ages%22,%20%22present%22:true},{name:%22Guilds%22,%20%22present%22:true},{name:%22Hinterlands%22,%20%22present%22:false},{name:%22Intrigue%22,%20present:true},{name:%22Promo%22,%20%22present%22:false},%20{%22name%22:%22Seaside%22,%20%22present%22:true}],%22rating%22:5%20}// http://localhost:8888/dominiondeck?action=GET&values={%22expansions%22:[%22Dark_Ages%22,%22Guilds%22,%22Hinterlands%22,%22Intrigue%22,%22Promo%22,%22Seaside%22]}
//
// Local Datastore:
// http://localhost:8888/_ah/admin/datastore

@SuppressWarnings("serial")
public class DominionDeckServlet extends HttpServlet {
	
	// action=RECORD&values={"deck":{"useShelters":false,"useColonyAndPlatinum":false,"kingdomPiles":["Haven","Hermit","Pirate Ship","Feodum","Death Cart","Junk Dealer","Ill-Gotten Gains","Merchant Guild","Stash","Nobles"],"events":[]},"expansions":["Dark Ages","Guilds","Hinterlands","Intrigue","Promo","Seaside"],"rating":5 }
	static public class JsonValues {
		JsonValues() {}
		
		Deck deck;
		Expansion[] expansions;
		Double rating;
		
		String debug;
	}
	
	static public class Deck {
		Deck() {}
		
		Boolean useShelters;
		Boolean useColonyAndPlatinum;
		String[] kingdomPiles;
		String[] events;
	}
	
	static public class Expansion {
		String name;
		Boolean present;
	}
		
	public void doGet(HttpServletRequest req, HttpServletResponse resp) throws IOException {
		resp.setContentType("text/plain");
		
		resp.setHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1
		resp.setHeader("Pragma", "no-cache"); // HTTP 1.0
		resp.setDateHeader("Expires", 0); // Proxies.
		
		Enumeration paramNames = req.getParameterNames();
		String action = req.getParameterValues("action")[0];
		String values = req.getParameterValues("values")[0];

		Gson gson = new Gson();
		JsonValues jsonValues = gson.fromJson(values, JsonValues.class);
		
		if (action.equals("RECORD")) {
			doRecordAction(jsonValues, resp.getWriter());
		} else if (action.equals("GET"))  {
			doGetAction(jsonValues, resp.getWriter());
		}
	}
	
	private Entity jsonValuesToEntity(JsonValues values) {
		Gson gson = new Gson();
		Entity record = new Entity("Record", gson.toJson(values.deck));
		for (Expansion expansion : values.expansions) {
			record.setProperty(expansion.name, expansion.present);
		}
		record.setProperty(getRatingKey(values), 1);
		record.setProperty("random", getRandomLong());
		
		return record;
	}
	
	private JsonValues entityToJsonValues(Entity entity) {
		Gson gson = new Gson();
		JsonValues values = new JsonValues();
		
		values.deck = gson.fromJson(entity.getKey().getName(), Deck.class);
		long total = 0;
		long count = 0;
		for (long rating = 1; rating <= 5; rating++) {
			long ratingCount = 0;
			String ratingKey = getRatingKey(rating);
			if (entity.hasProperty(ratingKey)) {
				ratingCount = (long) entity.getProperty(ratingKey);
			}
			count += ratingCount;
			total += ratingCount * rating;
		}
		values.rating = ((double) total)/count;
		
		return values;
	}
	
	private void doRecordAction(JsonValues values, PrintWriter writer) {
		DatastoreService datastore = DatastoreServiceFactory.getDatastoreService(); 
		Gson gson = new Gson();

		// First try to see if we've seen this deck before
		Filter keyFilter = new FilterPredicate(Entity.KEY_RESERVED_PROPERTY, 
				FilterOperator.EQUAL,
				KeyFactory.createKey("Record", gson.toJson(values.deck)));
		Query q =  new Query("Record").setFilter(keyFilter);
		PreparedQuery pq = datastore.prepare(q);

		int count = pq.countEntities(FetchOptions.Builder.withDefaults());
		if (count == 0) {
			// First time we've seen this deck
			datastore.put(jsonValuesToEntity(values));
		} else if (count == 1) {
			// We've seen this deck before
			Entity record = pq.asIterable().iterator().next();
			String ratingKey = getRatingKey(values);
			long ratingValue = 1;
			if (record.hasProperty(ratingKey)) {
				ratingValue = (long) record.getProperty(ratingKey) + 1;
			}
			record.setProperty(ratingKey, ratingValue);
			datastore.put(record);
		} else {
			throw new RuntimeException("deck present in store multiple times!");
		}
		
		writer.println("SUCCESS!");
	}
	
	private String getRatingKey(JsonValues values) {
		return getRatingKey(values.rating);
	}
	
	private String getRatingKey(double value) {
		return "rating_" +  Long.toString((long) value);
	}
	
	private void doGetAction(JsonValues values, PrintWriter writer) {
		DatastoreService datastore = DatastoreServiceFactory.getDatastoreService(); 
		Gson gson = new Gson();
		String log = new String();
		
		Filter keyFilter = new FilterPredicate("random",
				FilterOperator.GREATER_THAN_OR_EQUAL,
				getRandomLong());
		// TODO: fix filtering of expansions
		/*
		TreeSet<String> missingExpansions= new TreeSet<String>();
		for (Expansion expansion : values.expansions) {
			if (expansion.present == false) {
				Filter filter = new FilterPredicate(expansion.name,
						FilterOperator.NOT_EQUAL,
						true);
				keyFilter = CompositeFilterOperator.and(keyFilter, filter);	
				missingExpansions.add(expansion.name);
			}
		}
		writer.println(keyFilter.toString());
		*/
		Query q =  new Query("Record").setFilter(keyFilter).addSort("random", SortDirection.ASCENDING);
				
		/*
		q.addSort("random", SortDirection.ASCENDING);
		for (String missingExpansion : missingExpansions) {
			q.addSort(missingExpansion, SortDirection.ASCENDING);
		}
		*/
		
		PreparedQuery pq = datastore.prepare(q);
		Iterator<Entity> entityIterator = pq.asIterable().iterator();
		if (entityIterator.hasNext() == false) {
			q = new Query("Record").addSort("random", SortDirection.ASCENDING);
			pq = datastore.prepare(q);
			entityIterator = pq.asIterable().iterator();
			if (entityIterator.hasNext() == false) {
				throw new RuntimeException("no data in datastore");
			}			
		}
		Entity record = entityIterator.next();
		JsonValues resultValues = entityToJsonValues(record);
		resultValues.debug = log;
		String result = gson.toJson(resultValues);
		
		writer.println(result);
	}
	
	public long getRandomLong() {
		Random random = new Random();
		random.setSeed(new Date().getTime());
		return random.nextLong();
	}
}

--
-- PostgreSQL database dump
--

-- Dumped from database version 9.1.3
-- Dumped by pg_dump version 9.1.3
-- Started on 2012-08-03 16:24:43

SET statement_timeout = 0;
SET client_encoding = 'WIN1252';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- TOC entry 176 (class 3079 OID 11639)
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- TOC entry 2070 (class 0 OID 0)
-- Dependencies: 176
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


--
-- TOC entry 177 (class 3079 OID 18230)
-- Dependencies: 5
-- Name: citext; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS citext WITH SCHEMA public;


--
-- TOC entry 2071 (class 0 OID 0)
-- Dependencies: 177
-- Name: EXTENSION citext; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION citext IS 'data type for case-insensitive character strings';


SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 161 (class 1259 OID 18015)
-- Dependencies: 1988 5
-- Name: act; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE act (
    actname character varying(255) NOT NULL,
    comments character varying(8000) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public.act OWNER TO postgres;

--
-- TOC entry 162 (class 1259 OID 18024)
-- Dependencies: 1989 5
-- Name: artist; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE artist (
    artistname character varying(255) NOT NULL,
    comments character varying(8000) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public.artist OWNER TO postgres;

--
-- TOC entry 173 (class 1259 OID 18180)
-- Dependencies: 5
-- Name: artistinimage; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE artistinimage (
    imageid integer NOT NULL,
    artistname character varying(255) NOT NULL
);


ALTER TABLE public.artistinimage OWNER TO postgres;

--
-- TOC entry 175 (class 1259 OID 18210)
-- Dependencies: 5
-- Name: credit; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE credit (
    performancedate date NOT NULL,
    locationid character varying(3) NOT NULL,
    setno integer NOT NULL,
    pieceno integer NOT NULL,
    artistname character varying(255) NOT NULL,
    roleid character varying(3) NOT NULL
);


ALTER TABLE public.credit OWNER TO postgres;

--
-- TOC entry 172 (class 1259 OID 18160)
-- Dependencies: 2000 2001 2002 2003 5
-- Name: image; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE image (
    imageid integer NOT NULL,
    path character varying(511) NOT NULL,
    title character varying(255) DEFAULT ''::character varying NOT NULL,
    performancedate date DEFAULT '1900-01-01'::date NOT NULL,
    locationid character varying(3) DEFAULT ''::character varying NOT NULL,
    comments character varying(8000) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public.image OWNER TO postgres;

--
-- TOC entry 171 (class 1259 OID 18158)
-- Dependencies: 5 172
-- Name: image_imageid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE image_imageid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.image_imageid_seq OWNER TO postgres;

--
-- TOC entry 2077 (class 0 OID 0)
-- Dependencies: 171
-- Name: image_imageid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE image_imageid_seq OWNED BY image.imageid;


--
-- TOC entry 2078 (class 0 OID 0)
-- Dependencies: 171
-- Name: image_imageid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('image_imageid_seq', 39, true);


--
-- TOC entry 163 (class 1259 OID 18033)
-- Dependencies: 1990 5
-- Name: location; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE location (
    locationid character varying(3) NOT NULL,
    name character varying(255),
    comments character varying(8000) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public.location OWNER TO postgres;

--
-- TOC entry 164 (class 1259 OID 18044)
-- Dependencies: 5
-- Name: newsletter; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE newsletter (
    newsletterdate date NOT NULL,
    path character varying(511)
);


ALTER TABLE public.newsletter OWNER TO postgres;

--
-- TOC entry 168 (class 1259 OID 18077)
-- Dependencies: 1992 1993 5
-- Name: performance; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE performance (
    performancedate date NOT NULL,
    locationid character varying(3) NOT NULL,
    seriesid character varying(3) NOT NULL,
    newsletterdate date DEFAULT '1900-01-01'::date NOT NULL,
    comments character varying(8000) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public.performance OWNER TO postgres;

--
-- TOC entry 170 (class 1259 OID 18122)
-- Dependencies: 1996 1997 1998 5
-- Name: piece; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE piece (
    performancedate date NOT NULL,
    locationid character varying(3) NOT NULL,
    setno integer NOT NULL,
    pieceno integer NOT NULL,
    title character varying(255) NOT NULL,
    audiopath character varying(511) DEFAULT ''::character varying NOT NULL,
    videopath character varying(511) DEFAULT ''::character varying NOT NULL,
    comments character varying(8000) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public.piece OWNER TO postgres;

--
-- TOC entry 166 (class 1259 OID 18062)
-- Dependencies: 5
-- Name: role; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE role (
    roleid character varying(3) NOT NULL,
    name character varying(255)
);


ALTER TABLE public.role OWNER TO postgres;

--
-- TOC entry 165 (class 1259 OID 18051)
-- Dependencies: 1991 5
-- Name: series; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE series (
    seriesid character varying(3) NOT NULL,
    name character varying(255),
    comments character varying(8000) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public.series OWNER TO postgres;

--
-- TOC entry 169 (class 1259 OID 18102)
-- Dependencies: 1994 1995 5
-- Name: set; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE set (
    performancedate date NOT NULL,
    locationid character varying(3) NOT NULL,
    setno integer NOT NULL,
    actname character varying(255) DEFAULT ''::character varying NOT NULL,
    comments character varying(8000) DEFAULT ''::character varying NOT NULL
);


ALTER TABLE public.set OWNER TO postgres;

--
-- TOC entry 167 (class 1259 OID 18069)
-- Dependencies: 5
-- Name: useroption; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE useroption (
    userid character varying(255) NOT NULL,
    optionname character varying(225) NOT NULL,
    optionvalue character varying(8000) NOT NULL
);


ALTER TABLE public.useroption OWNER TO postgres;

--
-- TOC entry 174 (class 1259 OID 18196)
-- Dependencies: 1987 5
-- Name: vwperformance; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW vwperformance AS
    SELECT performance.performancedate AS date, location.name AS location, series.name AS series, performance.newsletterdate AS newsletter, performance.comments FROM ((performance JOIN location ON (((performance.locationid)::text = (location.locationid)::text))) JOIN series ON (((performance.seriesid)::text = (series.seriesid)::text)));


ALTER TABLE public.vwperformance OWNER TO postgres;

--
-- TOC entry 1999 (class 2604 OID 18163)
-- Dependencies: 171 172 172
-- Name: imageid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY image ALTER COLUMN imageid SET DEFAULT nextval('image_imageid_seq'::regclass);


--
-- TOC entry 2052 (class 0 OID 18015)
-- Dependencies: 161
-- Data for Name: act; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY act (actname, comments) FROM stdin;
Orchestra of the Spheres	Post galactic monk funk.
Duke Ellington	They loved him madly
CCC	CACd
Count Basie	Kansas City swig
BBB	rfgol;iSED SD.kjhs d dS ASlihS;oiHSD;oiSD ;oiSD;  oiS ZJSH  lihsd 
Ardvaark	
Zoro	
	
\.


--
-- TOC entry 2053 (class 0 OID 18024)
-- Dependencies: 162
-- Data for Name: artist; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY artist (artistname, comments) FROM stdin;
Julie Bevan	
Michael Hall	
Simon O'Rorke	http://simon.ororke.net/
Tom Callwood	
Chris Prosser	violin
Daniel Beban	xdfkljn  zdfljh dfljh zkjh z  Diurgh uriuiuzsd   zsd;oi.
\.


--
-- TOC entry 2063 (class 0 OID 18180)
-- Dependencies: 173
-- Data for Name: artistinimage; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY artistinimage (imageid, artistname) FROM stdin;
\.


--
-- TOC entry 2064 (class 0 OID 18210)
-- Dependencies: 175
-- Data for Name: credit; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY credit (performancedate, locationid, setno, pieceno, artistname, roleid) FROM stdin;
2010-10-01	FOW	1	1	Daniel Beban	eg
2010-10-01	FOW	1	1	Chris Prosser	vo
\.


--
-- TOC entry 2062 (class 0 OID 18160)
-- Dependencies: 172
-- Data for Name: image; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY image (imageid, path, title, performancedate, locationid, comments) FROM stdin;
10	pa	aaa	2010-10-01	FOW	fkjhvdd
15	pc	cc	2010-10-01	FOW	
14	pb	bb	2010-10-01	FOW	leirtgjsldiuh
16	pd	ddd	2012-03-22	FOW	no comment
17	pe	e	2012-03-21	HAP	
22	xdvkhnlk	fjkvhn	2012-03-21	HAP	
28	pi	i	2012-03-21	HAP	
24	pg	g	2012-03-21	HAP	
34	pk	k	2012-03-21	HAP	
32	pj	k	2012-03-21	HAP	
\.


--
-- TOC entry 2054 (class 0 OID 18033)
-- Dependencies: 163
-- Data for Name: location; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY location (locationid, name, comments) FROM stdin;
FRE	Fred's	
HAP	Happy	joy joy
EN	Enjoy Gallery	yum
FOW	Fowler	in our dreams
PHO	Photospace	
\.


--
-- TOC entry 2055 (class 0 OID 18044)
-- Dependencies: 164
-- Data for Name: newsletter; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY newsletter (newsletterdate, path) FROM stdin;
2012-03-16	C:\\Users\\Simon\\Documents\\Cosmos\\Hubble.gif
1900-01-01	
2012-05-18	C:\\Users\\Simon\\Documents\\Insured Items.doc
2012-02-09	C:\\Newsletters\\20120203.pdf
2012-05-27	jcv nf
2009-07-08	C:\\Users\\Simon\\Documents\\Cosmos\\hawk2.jpg
2012-05-28	dfkvjn
2009-08-07	C:\\Newsletters\\20090807.pdf
2010-05-19	C:\\Users\\Simon\\Documents\\bigbangt.doc
\.


--
-- TOC entry 2059 (class 0 OID 18077)
-- Dependencies: 168
-- Data for Name: performance; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY performance (performancedate, locationid, seriesid, newsletterdate, comments) FROM stdin;
2012-05-26	FRE	SUN	2012-03-16	
2010-10-01	FOW	SUN	1900-01-01	
2012-03-21	HAP		2012-05-27	uosipius
2012-03-22	FOW	SUN	2012-03-16	silukaslnasena askjisa
2012-04-24	PHO	F10	2009-07-08	ghjasld jd jj
\.


--
-- TOC entry 2061 (class 0 OID 18122)
-- Dependencies: 170
-- Data for Name: piece; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY piece (performancedate, locationid, setno, pieceno, title, audiopath, videopath, comments) FROM stdin;
2010-10-01	FOW	1	2	Black, Brown and Beige	aaa	ppp	
2010-10-01	FOW	1	3	aefuna awoihj aw			
2010-10-01	FOW	1	4	Black and Tan Fantasy	reesdec	dcdd	
2010-10-01	FOW	2	1	sdjhsd  sdjhngd d	dds	dvsd	
2010-10-01	FOW	2	2	fv ru ruh r1			
2010-10-01	FOW	2	3	rte5rt5t			
2010-10-01	FOW	1	1	Take the A Train	sssa	vvv	
2012-03-21	HAP	1	1	21 Mar 2012 Piece 1			
2012-03-21	HAP	1	2	21 Mar 2012 Piece 2			
2012-03-21	HAP	1	3	21 Mar 2012 Piece 3			
2012-03-21	HAP	1	4	fkvnh dui d			
2012-03-21	HAP	1	5	re iu euio 			
2010-10-01	FOW	1	5	fh riu iuer			
2010-10-01	FOW	1	6	rwi weiuh eui			
\.


--
-- TOC entry 2057 (class 0 OID 18062)
-- Dependencies: 166
-- Data for Name: role; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY role (roleid, name) FROM stdin;
ag	acoustic guitar
bg	bass guitar
ct	clarinet
d	drums
db	double bass
eg	electric guitar
ng	nylon-string guitar
pc	percussion
ts	tenor saxophone
ss	soprano saxophone
sy	synth
vn	violin
vo	voice
va	viola
tp	trumpet
tb	trombone
\.


--
-- TOC entry 2056 (class 0 OID 18051)
-- Dependencies: 165
-- Data for Name: series; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY series (seriesid, name, comments) FROM stdin;
F09	Fredstock 2009	A good time was had by all
F10	Fredstock 2010	
SUN	Sunday Night Variety	
		
\.


--
-- TOC entry 2060 (class 0 OID 18102)
-- Dependencies: 169
-- Data for Name: set; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY set (performancedate, locationid, setno, actname, comments) FROM stdin;
2010-10-01	FOW	2	Count Basie	
2012-03-21	HAP	1		
2012-03-21	HAP	3	Ardvaark	fdkvddd
2010-10-01	FOW	1	Duke Ellington	dkcvj
\.


--
-- TOC entry 2058 (class 0 OID 18069)
-- Dependencies: 167
-- Data for Name: useroption; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY useroption (userid, optionname, optionvalue) FROM stdin;
Simon	MdiParentForm.WindowState	0
Simon	TableForm.Left	0
Simon	TableForm.Top	0
Simon	TableForm.WindowState	2
Simon	Table	Location
Simon	TableForm.Height	360
Simon	MdiParentForm.Left	472
Simon	MdiParentForm.Top	13
Simon	TableForm.Width	395
Simon	SplitterDistance	201
Simon	MdiParentForm.Height	692
Simon	MdiParentForm.Width	1041
\.


--
-- TOC entry 2005 (class 2606 OID 18023)
-- Dependencies: 161 161
-- Name: pk_act; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY act
    ADD CONSTRAINT pk_act PRIMARY KEY (actname);


--
-- TOC entry 2007 (class 2606 OID 18032)
-- Dependencies: 162 162
-- Name: pk_artist; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY artist
    ADD CONSTRAINT pk_artist PRIMARY KEY (artistname);


--
-- TOC entry 2037 (class 2606 OID 18184)
-- Dependencies: 173 173 173
-- Name: pk_artistinimage; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY artistinimage
    ADD CONSTRAINT pk_artistinimage PRIMARY KEY (imageid, artistname);


--
-- TOC entry 2039 (class 2606 OID 18214)
-- Dependencies: 175 175 175 175 175 175 175
-- Name: pk_credit; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY credit
    ADD CONSTRAINT pk_credit PRIMARY KEY (performancedate, locationid, setno, pieceno, artistname, roleid);


--
-- TOC entry 2033 (class 2606 OID 18172)
-- Dependencies: 172 172
-- Name: pk_image; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY image
    ADD CONSTRAINT pk_image PRIMARY KEY (imageid);


--
-- TOC entry 2009 (class 2606 OID 18041)
-- Dependencies: 163 163
-- Name: pk_location; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY location
    ADD CONSTRAINT pk_location PRIMARY KEY (locationid);


--
-- TOC entry 2013 (class 2606 OID 18048)
-- Dependencies: 164 164
-- Name: pk_newsletter; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY newsletter
    ADD CONSTRAINT pk_newsletter PRIMARY KEY (newsletterdate);


--
-- TOC entry 2027 (class 2606 OID 18086)
-- Dependencies: 168 168 168
-- Name: pk_performance; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY performance
    ADD CONSTRAINT pk_performance PRIMARY KEY (performancedate, locationid);


--
-- TOC entry 2031 (class 2606 OID 18132)
-- Dependencies: 170 170 170 170 170
-- Name: pk_piece; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY piece
    ADD CONSTRAINT pk_piece PRIMARY KEY (performancedate, locationid, setno, pieceno);


--
-- TOC entry 2021 (class 2606 OID 18066)
-- Dependencies: 166 166
-- Name: pk_role; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY role
    ADD CONSTRAINT pk_role PRIMARY KEY (roleid);


--
-- TOC entry 2017 (class 2606 OID 18059)
-- Dependencies: 165 165
-- Name: pk_series; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY series
    ADD CONSTRAINT pk_series PRIMARY KEY (seriesid);


--
-- TOC entry 2029 (class 2606 OID 18111)
-- Dependencies: 169 169 169 169
-- Name: pk_set; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY set
    ADD CONSTRAINT pk_set PRIMARY KEY (performancedate, locationid, setno);


--
-- TOC entry 2025 (class 2606 OID 18076)
-- Dependencies: 167 167 167
-- Name: pk_useroption; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY useroption
    ADD CONSTRAINT pk_useroption PRIMARY KEY (userid, optionname);


--
-- TOC entry 2035 (class 2606 OID 18174)
-- Dependencies: 172 172
-- Name: uk_image_path; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY image
    ADD CONSTRAINT uk_image_path UNIQUE (path);


--
-- TOC entry 2011 (class 2606 OID 18043)
-- Dependencies: 163 163
-- Name: uk_location_name; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY location
    ADD CONSTRAINT uk_location_name UNIQUE (name);


--
-- TOC entry 2015 (class 2606 OID 18050)
-- Dependencies: 164 164
-- Name: uk_newsletter_path; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY newsletter
    ADD CONSTRAINT uk_newsletter_path UNIQUE (path);


--
-- TOC entry 2023 (class 2606 OID 18068)
-- Dependencies: 166 166
-- Name: uk_role_name; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY role
    ADD CONSTRAINT uk_role_name UNIQUE (name);


--
-- TOC entry 2019 (class 2606 OID 18061)
-- Dependencies: 165 165
-- Name: uk_series_name; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY series
    ADD CONSTRAINT uk_series_name UNIQUE (name);


--
-- TOC entry 2047 (class 2606 OID 18185)
-- Dependencies: 173 162 2006
-- Name: fk_artistinimage_artist; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY artistinimage
    ADD CONSTRAINT fk_artistinimage_artist FOREIGN KEY (artistname) REFERENCES artist(artistname) ON UPDATE CASCADE;


--
-- TOC entry 2048 (class 2606 OID 18190)
-- Dependencies: 172 2032 173
-- Name: fk_artistinimage_image; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY artistinimage
    ADD CONSTRAINT fk_artistinimage_image FOREIGN KEY (imageid) REFERENCES image(imageid) ON UPDATE CASCADE;


--
-- TOC entry 2049 (class 2606 OID 18215)
-- Dependencies: 2006 175 162
-- Name: fk_credit_artist; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY credit
    ADD CONSTRAINT fk_credit_artist FOREIGN KEY (artistname) REFERENCES artist(artistname) ON UPDATE CASCADE;


--
-- TOC entry 2051 (class 2606 OID 18225)
-- Dependencies: 175 175 175 175 170 170 170 170 2030
-- Name: fk_credit_piece; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY credit
    ADD CONSTRAINT fk_credit_piece FOREIGN KEY (performancedate, locationid, setno, pieceno) REFERENCES piece(performancedate, locationid, setno, pieceno) ON UPDATE CASCADE;


--
-- TOC entry 2050 (class 2606 OID 18220)
-- Dependencies: 175 166 2020
-- Name: fk_credit_role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY credit
    ADD CONSTRAINT fk_credit_role FOREIGN KEY (roleid) REFERENCES role(roleid) ON UPDATE CASCADE;


--
-- TOC entry 2046 (class 2606 OID 18175)
-- Dependencies: 168 2026 172 172 168
-- Name: fk_image_performance; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY image
    ADD CONSTRAINT fk_image_performance FOREIGN KEY (performancedate, locationid) REFERENCES performance(performancedate, locationid) ON UPDATE CASCADE;


--
-- TOC entry 2040 (class 2606 OID 18087)
-- Dependencies: 163 168 2008
-- Name: fk_performance_location; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY performance
    ADD CONSTRAINT fk_performance_location FOREIGN KEY (locationid) REFERENCES location(locationid) ON UPDATE CASCADE;


--
-- TOC entry 2042 (class 2606 OID 18097)
-- Dependencies: 164 168 2012
-- Name: fk_performance_newsletter; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY performance
    ADD CONSTRAINT fk_performance_newsletter FOREIGN KEY (newsletterdate) REFERENCES newsletter(newsletterdate) ON UPDATE CASCADE;


--
-- TOC entry 2041 (class 2606 OID 18092)
-- Dependencies: 2016 165 168
-- Name: fk_performance_series; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY performance
    ADD CONSTRAINT fk_performance_series FOREIGN KEY (seriesid) REFERENCES series(seriesid) ON UPDATE CASCADE;


--
-- TOC entry 2045 (class 2606 OID 18133)
-- Dependencies: 2028 169 170 169 169 170 170
-- Name: fk_piece_set; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY piece
    ADD CONSTRAINT fk_piece_set FOREIGN KEY (performancedate, locationid, setno) REFERENCES set(performancedate, locationid, setno) ON UPDATE CASCADE;


--
-- TOC entry 2044 (class 2606 OID 18117)
-- Dependencies: 161 2004 169
-- Name: fk_set_act; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY set
    ADD CONSTRAINT fk_set_act FOREIGN KEY (actname) REFERENCES act(actname) ON UPDATE CASCADE;


--
-- TOC entry 2043 (class 2606 OID 18112)
-- Dependencies: 2026 169 169 168 168
-- Name: fk_set_performance; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY set
    ADD CONSTRAINT fk_set_performance FOREIGN KEY (performancedate, locationid) REFERENCES performance(performancedate, locationid) ON UPDATE CASCADE;


--
-- TOC entry 2069 (class 0 OID 0)
-- Dependencies: 5
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- TOC entry 2072 (class 0 OID 0)
-- Dependencies: 161
-- Name: act; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE act FROM PUBLIC;
REVOKE ALL ON TABLE act FROM postgres;
GRANT ALL ON TABLE act TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE act TO fred;


--
-- TOC entry 2073 (class 0 OID 0)
-- Dependencies: 162
-- Name: artist; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE artist FROM PUBLIC;
REVOKE ALL ON TABLE artist FROM postgres;
GRANT ALL ON TABLE artist TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE artist TO fred;


--
-- TOC entry 2074 (class 0 OID 0)
-- Dependencies: 173
-- Name: artistinimage; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE artistinimage FROM PUBLIC;
REVOKE ALL ON TABLE artistinimage FROM postgres;
GRANT ALL ON TABLE artistinimage TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE artistinimage TO fred;


--
-- TOC entry 2075 (class 0 OID 0)
-- Dependencies: 175
-- Name: credit; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE credit FROM PUBLIC;
REVOKE ALL ON TABLE credit FROM postgres;
GRANT ALL ON TABLE credit TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE credit TO fred;


--
-- TOC entry 2076 (class 0 OID 0)
-- Dependencies: 172
-- Name: image; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE image FROM PUBLIC;
REVOKE ALL ON TABLE image FROM postgres;
GRANT ALL ON TABLE image TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE image TO fred;


--
-- TOC entry 2079 (class 0 OID 0)
-- Dependencies: 171
-- Name: image_imageid_seq; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON SEQUENCE image_imageid_seq FROM PUBLIC;
REVOKE ALL ON SEQUENCE image_imageid_seq FROM postgres;
GRANT ALL ON SEQUENCE image_imageid_seq TO postgres;
GRANT USAGE ON SEQUENCE image_imageid_seq TO fred;


--
-- TOC entry 2080 (class 0 OID 0)
-- Dependencies: 163
-- Name: location; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE location FROM PUBLIC;
REVOKE ALL ON TABLE location FROM postgres;
GRANT ALL ON TABLE location TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE location TO fred;


--
-- TOC entry 2081 (class 0 OID 0)
-- Dependencies: 164
-- Name: newsletter; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE newsletter FROM PUBLIC;
REVOKE ALL ON TABLE newsletter FROM postgres;
GRANT ALL ON TABLE newsletter TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE newsletter TO fred;


--
-- TOC entry 2082 (class 0 OID 0)
-- Dependencies: 168
-- Name: performance; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE performance FROM PUBLIC;
REVOKE ALL ON TABLE performance FROM postgres;
GRANT ALL ON TABLE performance TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE performance TO fred;


--
-- TOC entry 2083 (class 0 OID 0)
-- Dependencies: 170
-- Name: piece; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE piece FROM PUBLIC;
REVOKE ALL ON TABLE piece FROM postgres;
GRANT ALL ON TABLE piece TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE piece TO fred;


--
-- TOC entry 2084 (class 0 OID 0)
-- Dependencies: 166
-- Name: role; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE role FROM PUBLIC;
REVOKE ALL ON TABLE role FROM postgres;
GRANT ALL ON TABLE role TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE role TO fred;


--
-- TOC entry 2085 (class 0 OID 0)
-- Dependencies: 165
-- Name: series; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE series FROM PUBLIC;
REVOKE ALL ON TABLE series FROM postgres;
GRANT ALL ON TABLE series TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE series TO fred;


--
-- TOC entry 2086 (class 0 OID 0)
-- Dependencies: 169
-- Name: set; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE set FROM PUBLIC;
REVOKE ALL ON TABLE set FROM postgres;
GRANT ALL ON TABLE set TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE set TO fred;


--
-- TOC entry 2087 (class 0 OID 0)
-- Dependencies: 167
-- Name: useroption; Type: ACL; Schema: public; Owner: postgres
--

REVOKE ALL ON TABLE useroption FROM PUBLIC;
REVOKE ALL ON TABLE useroption FROM postgres;
GRANT ALL ON TABLE useroption TO postgres;
GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE useroption TO fred;


-- Completed on 2012-08-03 16:24:44

--
-- PostgreSQL database dump complete
--


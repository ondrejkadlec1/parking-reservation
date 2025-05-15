ALTER DATABASE parking_reservation SET timezone TO 'Europe/Prague';

DROP TABLE IF EXISTS public.reservations;

CREATE TABLE IF NOT EXISTS public.reservations
(
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    state_id integer NOT NULL DEFAULT 1,
    space_number integer NOT NULL,
    type_id integer NOT NULL,
    begins_at timestamp(0) with time zone NOT NULL,
    ends_at timestamp(0) with time zone NOT NULL,
    created_at timestamp(2) with time zone NOT NULL DEFAULT NOW(),
    user_id uuid NOT NULL,
    comment varchar(256),
    PRIMARY KEY (id)
);

DROP TABLE IF EXISTS public.spaces;

CREATE TABLE IF NOT EXISTS public.spaces
(
    space_number serial NOT NULL,
    created_by uuid NOT NULL,
    created_at timestamp(2) with time zone NOT NULL DEFAULT NOW(),
    PRIMARY KEY (space_number)
);


DROP TABLE IF EXISTS public.states;

CREATE TABLE IF NOT EXISTS public.states
(
    id serial NOT NULL,
    name varchar(64) NOT NULL,
    name_cs varchar(64) NOT NULL,
    PRIMARY KEY (id)
);

DROP TABLE IF EXISTS public.reservation_types;

CREATE TABLE IF NOT EXISTS public.reservation_types
(
    id serial NOT NULL,
    name varchar(64) NOT NULL,
    PRIMARY KEY (id)
);

DROP TABLE IF EXISTS public.users;

CREATE TABLE IF NOT EXISTS public.users
(
    id uuid NOT NULL,
    added_at timestamp(2) with time zone NOT NULL DEFAULT NOW(),
    is_active bool NOT NULL DEFAULT true,
    is_admin bool NOT NULL DEFAULT false,
    display_name varchar(64) NOT NULL,
    PRIMARY KEY (id),
	UNIQUE (id)
);


CREATE INDEX index_spn_reservations ON public.reservations (space_number);
CREATE INDEX index_usr_reservations ON public.reservations (user_id);


ALTER TABLE IF EXISTS public.reservations
    ADD FOREIGN KEY (state_id)
    REFERENCES public.states (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE RESTRICT
    NOT VALID;


ALTER TABLE IF EXISTS public.reservations
    ADD FOREIGN KEY (type_id)
    REFERENCES public.reservation_types (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE RESTRICT
    NOT VALID;


ALTER TABLE IF EXISTS public.reservations
    ADD FOREIGN KEY (space_number)
    REFERENCES public.spaces (space_number) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.reservations
    ADD FOREIGN KEY (user_id)
    REFERENCES public.users (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE
    NOT VALID;


ALTER TABLE IF EXISTS public.spaces
    ADD FOREIGN KEY (created_by)
    REFERENCES public.users (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE RESTRICT
    NOT VALID;


CREATE OR REPLACE FUNCTION is_time_rounded(ts TIMESTAMP WITH TIME ZONE)
    RETURNS BOOLEAN 
    LANGUAGE plpgsql
    AS $$
    BEGIN
        RETURN EXTRACT(SECONDS FROM ts) = 0
        AND EXTRACT(MINUTES FROM ts) IN (0, 30);
    END; $$;

ALTER TABLE IF EXISTS public.reservations
    ADD CONSTRAINT rnd_tm_reservations CHECK (is_time_rounded(begins_at) and is_time_rounded(ends_at));


INSERT INTO public.states (name, name_cs) VALUES 
    ('undecided', 'nerozhodnuto'), 
    ('confirmed', 'potvrzeno'), 
    ('canceled', 'zrušeno');

INSERT INTO public.reservation_types (name) VALUES ('normal'), ('blocking');
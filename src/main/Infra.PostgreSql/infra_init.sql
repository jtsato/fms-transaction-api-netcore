CREATE FUNCTION public.process_table_audit() RETURNS trigger
    LANGUAGE plpgsql
AS $_$
begin
    IF (TG_OP = 'DELETE') then
        EXECUTE 'INSERT INTO ' || TG_TABLE_NAME || '_history SELECT nextval('''|| TG_TABLE_NAME || '_history_id_seq'') ,'''|| TG_OP ||''', now(), $1.*' USING OLD;
RETURN OLD;
ELSIF (TG_OP = 'UPDATE') then
        EXECUTE 'INSERT INTO ' || TG_TABLE_NAME || '_history SELECT nextval('''|| TG_TABLE_NAME || '_history_id_seq'') ,'''|| TG_OP ||''', now(), $1.*' USING NEW;
RETURN NEW;
ELSIF (TG_OP = 'INSERT') then
        EXECUTE 'INSERT INTO ' || TG_TABLE_NAME || '_history SELECT nextval('''|| TG_TABLE_NAME || '_history_id_seq'') ,'''|| TG_OP ||''', now(), $1.*' USING NEW;
RETURN NEW;
END IF;
RETURN NULL;
END;
$_$;

CREATE FUNCTION public.audit_notify_channel() RETURNS trigger
    LANGUAGE plpgsql
AS $$
DECLARE
payload TEXT;
BEGIN
    IF (TG_OP = 'DELETE') then
        payload := json_build_object('timestamp',CURRENT_TIMESTAMP,'action',TG_OP,'entity',TG_TABLE_NAME,'id',OLD.id,'log',row_to_json(OLD));
ELSE
        payload := json_build_object('timestamp',CURRENT_TIMESTAMP,'action',TG_OP,'entity',TG_TABLE_NAME,'id',NEW.id,'log',row_to_json(NEW));
END IF;
    -- Build the payload

EXECUTE 'INSERT INTO logs_audit (payload) VALUES (''' || encode(convert_to(payload, 'UTF-8'), 'base64') || ''')';

RETURN NULL;
END;
$$;

CREATE TABLE public.projects (
                                 id integer NOT NULL,
                                 name character varying(255) NOT NULL,
                                 problem_solved text,
                                 objective text,
                                 scope text,
                                 premises text,
                                 process_change boolean,
                                 criticality_id integer,
                                 priority integer,
                                 capex boolean,
                                 opex boolean,
                                 estimated_cost numeric(19,2),
                                 is_special boolean,
                                 status integer,
                                 created_at timestamp(0) without time zone,
                                 updated_at timestamp(0) without time zone,
                                 "user" integer NOT NULL,
                                 bc_flag boolean,
                                 bc_file text,
                                 workflow_id integer DEFAULT 1 NOT NULL,
                                 areas_id bigint,
                                 stakeholders_id integer,
                                 transformation_managers_id integer,
                                 refused_cause text,
                                 refused_date date,
                                 refused_user bigint,
                                 id_jira character varying(15),
                                 investment_order integer,
                                 bc_file_name text,
                                 strategic_indicators_id integer,
                                 summary character varying(255),
                                 score double precision,
                                 bc_status_id integer,
                                 current_step_id bigint DEFAULT '1'::bigint NOT NULL,
                                 created_user integer DEFAULT 1 NOT NULL
);

CREATE INDEX ix_projects_areas_id ON public.projects USING btree (areas_id);
CREATE INDEX ix_projects_workflow_id ON public.projects USING btree (workflow_id);

CREATE TRIGGER projects_audit AFTER INSERT OR DELETE OR UPDATE ON public.projects FOR EACH ROW EXECUTE PROCEDURE public.process_table_audit();
CREATE TRIGGER projects_notify_channel AFTER INSERT OR DELETE OR UPDATE ON public.projects FOR EACH ROW EXECUTE PROCEDURE public.audit_notify_channel();

CREATE TABLE public.subprojects (
                                    id bigint NOT NULL,
                                    project_id integer NOT NULL,
                                    name character varying(255) NOT NULL,
                                    delivery text,
                                    problems text,
                                    macro_scope text,
                                    acceptance_criteria text,
                                    out_scope text,
                                    premises text,
                                    restrictions text,
                                    score numeric(8,2) NOT NULL,
                                    status smallint NOT NULL,
                                    "user" integer NOT NULL,
                                    created_at timestamp(0) without time zone,
                                    updated_at timestamp(0) without time zone,
                                    is_modernization boolean DEFAULT false NOT NULL,
                                    start_quarter_id integer,
                                    end_quarter_id integer
);

CREATE INDEX subprojects_project_id_idx ON public.subprojects USING btree (project_id);

CREATE TRIGGER subproject_audit AFTER INSERT OR DELETE OR UPDATE ON public.subprojects FOR EACH ROW EXECUTE PROCEDURE public.process_table_audit();
CREATE TRIGGER subprojects_notify_channel AFTER INSERT OR DELETE OR UPDATE ON public.subprojects FOR EACH ROW EXECUTE PROCEDURE public.audit_notify_channel();

CREATE TABLE public.epics (
                              id integer NOT NULL,
                              project_id integer,
                              stage_id integer NOT NULL,
                              id_jira character varying(255),
                              name character varying(300) NOT NULL,
                              priority integer,
                              status_id integer DEFAULT 1 NOT NULL,
                              change_office_manager character varying(255),
                              delivery_baseline date,
                              delivery_planned date,
                              delivery_done date,
                              assisted_operation_baseline date,
                              assisted_operation_planned date,
                              rollout_baseline date,
                              rollout_planned date,
                              rollout_percentage numeric(8,2),
                              created_at timestamp(0) without time zone,
                              updated_at timestamp(0) without time zone,
                              "user" integer NOT NULL,
                              ppm character varying(250),
                              vsm_id integer,
                              gpm_id integer,
                              journeys_id integer,
                              tribes_id integer,
                              epic_description text,
                              squad_key character varying(10),
                              black_friday boolean DEFAULT false NOT NULL,
                              demanding_area integer,
                              previous_status_id integer,
                              previous_status_id_data timestamp(0) without time zone,
                              progress double precision DEFAULT '0'::double precision NOT NULL,
                              date_jira_updated timestamp(0) without time zone,
                              jira_date_success timestamp(0) without time zone,
                              jira_error_message character varying(255),
                              integration_id integer DEFAULT 1 NOT NULL,
                              integration_user_id integer,
                              integration_updated_at timestamp(0) without time zone,
                              subproject_id integer,
                              created_user integer DEFAULT 1 NOT NULL,
                              capex boolean DEFAULT false NOT NULL,
                              opex boolean DEFAULT false NOT NULL,
                              epic_type_id integer,
                              stakeholder_id integer,
                              epic_responsible_id integer,
                              jira_already_integrated boolean,
                              senior_gpm_id integer,
                              expected_result text,
                              measured_result text,
                              is_modernization boolean DEFAULT false NOT NULL
);

CREATE INDEX epics_subproject_id_idx ON public.epics USING btree (subproject_id);
CREATE INDEX ix_epics_id_jira ON public.epics USING btree (id_jira);
CREATE INDEX ix_epics_project_id_stage_id ON public.epics USING btree (project_id, stage_id);

CREATE TRIGGER epics_audit AFTER INSERT OR DELETE OR UPDATE ON public.epics FOR EACH ROW EXECUTE PROCEDURE public.process_table_audit();
CREATE TRIGGER epics_notify_channel AFTER INSERT OR DELETE OR UPDATE ON public.epics FOR EACH ROW EXECUTE PROCEDURE public.audit_notify_channel();
